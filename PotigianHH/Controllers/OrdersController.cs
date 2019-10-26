using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PotigianHH.Controllers.Model;
using PotigianHH.Database;
using PotigianHH.Model;

namespace PotigianHH.Controllers
{
    [Route("api/ordenes")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly PotigianContext potigianContext;

        public OrdersController(PotigianContext potigianContext)
        {
            this.potigianContext = potigianContext;
        }

        [HttpGet("proveedor/{code}")]
        public async Task<ActionResult<Response<List<PurchaseOrderHeader>>>> GetOrdersByProvider(int code)
        {
            return await RequestsHandler.HandleAsyncRequest(
                async () => await potigianContext.PurchaseOrdersHeaders.Where(o => o.ProviderCode == code && o.Situation == 1).ToListAsync());
        }

        [HttpGet("{suffixCode}/proveedor/{providerCode}")]
        public async Task<ActionResult<Response<List<PurchaseOrderHeader>>>> GetOrdersByProviderAndSuffixCode(string providerCode, string suffixCode)
        {
            return await RequestsHandler.HandleAsyncRequest(
                async () =>
                {
                    var expr = default(Expression<Func<PurchaseOrderHeader, bool>>);
                    if (providerCode == "none" && suffixCode == "none")
                    {
                        expr = o => o.Situation == 1;
                    }
                    else if (providerCode == "none")
                    {
                        expr = o => o.OrderSuffix.ToString().Contains(suffixCode) && o.Situation == 1;
                    }
                    else if (suffixCode == "none")
                    {
                        expr = o => o.ProviderCode.ToString().Contains(providerCode) && o.Situation == 1;
                    }
                    else
                    {
                        expr = o => o.OrderSuffix.ToString().Contains(suffixCode) && o.ProviderCode.ToString().Contains(providerCode) && o.Situation == 1;
                    }

                    var ocs = potigianContext.PurchaseOrdersHeaders.Where(expr);

                    await ocs.ForEachAsync(async oc =>
                    {
                        oc.Items = await potigianContext.PurchaseOrderDetails
                            .Where(od => od.SuffixOcCode == oc.OrderSuffix && od.PrefixOcCode == oc.OrderPrefix && od.OcCode == oc.OrderCode)
                            .CountAsync();
                    });

                    return await ocs.ToListAsync();
                });
        }

        [HttpGet("{prefixCode}/{ocCode}/{suffixCode}")]
        public async Task<ActionResult<Response<List<PurchaseOrderDetails>>>> GetOrderDetails(string prefixCode, string ocCode, string suffixCode)
        {
            return await RequestsHandler.HandleAsyncRequest(
                async () => await potigianContext.PurchaseOrderDetails
                    .Where(od =>
                        od.PrefixOcCode.ToString() == prefixCode &&
                        od.OcCode.ToString() == ocCode &&
                        od.SuffixOcCode.ToString() == suffixCode)
                    .Join(
                        potigianContext.Articles,
                        od => od.ArticleCode,
                        article => article.Code,
                        (od, article) => od.Append(article))
                    .Join(
                        potigianContext.ProviderArticles,
                        od => od.ArticleCode,
                        providerArticle => providerArticle.ArticleCode,
                        (od, providerArticle) => od.Append(providerArticle))
                    .ToListAsync());
        }

        [HttpPost("{prefixCode}/{ocCode}/{suffixCode}")]
        public async Task<ActionResult<Response<SynchronizeOrderResponse>>> SynchronizeOrder(string prefixCode, string ocCode, string suffixCode, [FromBody] SynchronizeOrderPayload payload)
        {
            return await RequestsHandler.HandleAsyncRequest(
                async () =>
                {
                    var orderHeader = await potigianContext.PurchaseOrdersHeaders
                        .FirstAsync(h => h.OrderPrefix == decimal.Parse(prefixCode) && h.OrderSuffix == decimal.Parse(suffixCode) && h.OrderCode == decimal.Parse(ocCode));
                    var orderDetails = await potigianContext.PurchaseOrderDetails
                        .Where(d => d.PrefixOcCode == decimal.Parse(prefixCode) && d.SuffixOcCode == decimal.Parse(suffixCode) && d.OcCode == decimal.Parse(ocCode))
                        .ToListAsync();
                    var articleCodes = orderDetails.Select(d => d.ArticleCode);
                    var articles = await potigianContext.Articles.Where(a => articleCodes.Contains(a.Code)).ToListAsync();
                    var detailsPending = orderDetails.Where(d => d.RequestedPackages > payload.ArticleCount[d.ArticleCode]);

                    await potigianContext.Database.BeginTransactionAsync();

                    var response = new SynchronizeOrderResponse();

                    // CABE Procedure
                    {
                        var prefixParam = new SqlParameter("@U_PREFIJO_OC", value: decimal.Parse(prefixCode));
                        var suffixParam = new SqlParameter("@U_SUFIJO_OC", value: decimal.Parse(suffixCode));
                        var situationParam = new SqlParameter("@C_SITUAC_OC", value: (decimal)(detailsPending.Any() ? 1 : 2)); // 1 pending - 2 ready
                        var receivedGoodsDateParam = new SqlParameter("@f_comp_ing_merc", value: payload.OrderAdditionalInfo.Date);
                        var receivedGoodsCodeParam = new SqlParameter("@C_COMP_ING_MERC", value: payload.OrderAdditionalInfo.BillType);
                        var purchasePrefixParam = new SqlParameter("@U_PREFIJO_COMP_ING_MERC", value: payload.OrderAdditionalInfo.PrefixOc);
                        var purchaseSuffixParam = new SqlParameter("@U_SUFIJO_COMP_ING_MERC", value: payload.OrderAdditionalInfo.SuffixOc);
                        var observationParam = new SqlParameter("@D_OBSERVACION_ING_MERC", value: payload.OrderAdditionalInfo.Observations);
                        var userParam = new SqlParameter("@vchUsuario", value: payload.OrderAdditionalInfo.User);
                        var terminalParam = new SqlParameter("@vchTerminal", value: payload.OrderAdditionalInfo.Terminal);
                        var outputMessageParam = new SqlParameter
                        {
                            ParameterName = "@vchMensaje",
                            Value = "",
                            Direction = ParameterDirection.Output,
                            SqlDbType = SqlDbType.VarChar,
                            Size = 255
                        };
                        var returnCode = new SqlParameter
                        {
                            ParameterName = "@returnCode",
                            Value = -1,
                            Direction = ParameterDirection.Output,
                        };

                        await potigianContext.Database.ExecuteSqlCommandAsync("EXEC @returnCode = dbo.SD03_OC_ING_MERC_ACTU_CABE " +
                            "@U_PREFIJO_OC, @U_SUFIJO_OC, @C_SITUAC_OC, @f_comp_ing_merc, @C_COMP_ING_MERC, " +
                            "@U_PREFIJO_COMP_ING_MERC, @U_SUFIJO_COMP_ING_MERC, @D_OBSERVACION_ING_MERC, " +
                            "@vchUsuario, @vchTerminal, @vchMensaje OUTPUT", returnCode, prefixParam, suffixParam, situationParam,
                            receivedGoodsDateParam, receivedGoodsCodeParam, purchasePrefixParam, purchaseSuffixParam,
                            observationParam, userParam, terminalParam, outputMessageParam);

                        response.HeaderMessage = (string) outputMessageParam.Value;
                        response.HeaderReturnCode = (int) returnCode.Value;
                    }

                    // DETA PROCEDURE
                    foreach (var orderDetail in orderDetails)
                    {
                        var ocParam = new SqlParameter("@C_OC", value: decimal.Parse(ocCode));
                        var companyBranchParam = new SqlParameter("@C_SUCU_EMPR", value: payload.OrderAdditionalInfo.Branch);
                        var destinationBranchParam = new SqlParameter("@C_SUCU_DESTINO_ALT", value: orderHeader.AlternativeDestinationBranch);
                        var prefixOcParam = new SqlParameter("@U_PREFIJO_OC", value: decimal.Parse(prefixCode));
                        var suffixOcParam = new SqlParameter("@U_SUFIJO_OC", value: decimal.Parse(suffixCode));
                        var ocMotherParam = new SqlParameter("@M_OC_MADRE", value: orderHeader.MotherFlag);
                        var situationCodeParam = new SqlParameter("@C_SITUAC_OC", value: (decimal)(detailsPending.Any(d => d.ArticleCode == orderDetail.ArticleCode) ? 1 : 2)); // 1 pending - 2 ready
                        var articleCodeParam = new SqlParameter("@C_ARTICULO", value: orderDetail.ArticleCode);
                        var saleByWeightParam = new SqlParameter("@M_VENDE_POR_PESO", value: articles.First(a => a.Code == orderDetail.ArticleCode).SellByWeight);
                        var requestedUnitsParam = new SqlParameter("@Q_UNID_KGS_PED", value: orderDetail.RequestedPackages);
                        var deliveredUnitsParam = new SqlParameter("@Q_UNID_KGS_CUMPL", value: payload.ArticleCount[orderDetail.ArticleCode]);
                        var pendingUnitsParam = new SqlParameter("@Q_UNID_KGS_PEND", value: orderDetail.RequestedPackages - payload.ArticleCount[orderDetail.ArticleCode]);
                        var receivedUnitsParam = new SqlParameter("@Q_UNID_KGS_ING", value: orderDetail.RequestedPackages);
                        var receivedPacksParam = new SqlParameter("@Q_BULTOS_KGS_ING", value: orderDetail.RequestedBulks);
                        var receivedPiecesFactorParam = new SqlParameter("@Q_FACTOR_PIEZAS_ING", value: orderDetail.RequestedFactor);
                        var receivedGoodsBoughtParam = new SqlParameter("@C_COMP_ING_MERC", value: payload.OrderAdditionalInfo.BillType);
                        var receivedGoodsPrefixParam = new SqlParameter("@U_PREFIJO_COMP_ING_MERC", value: payload.OrderAdditionalInfo.PrefixOc);
                        var receivedGoodsSuffixParam = new SqlParameter("@U_SUFIJO_COMP_ING_MERC", value: payload.OrderAdditionalInfo.SuffixOc);
                        var goodsDateParam = new SqlParameter("@F_COMP_ING_MERC", value: payload.OrderAdditionalInfo.Date);
                        var programCodeParam = new SqlParameter("@C_PROGRAMA", value: Config.ProgramCode);
                        var observationParam = new SqlParameter("@D_OBSERVACION", value: payload.OrderAdditionalInfo.Observations);
                        var userParam = new SqlParameter("@vchUsuario", value: payload.OrderAdditionalInfo.User);
                        var terminalParam = new SqlParameter("@vchTerminal", value: payload.OrderAdditionalInfo.Terminal);
                        var outputMessageParam = new SqlParameter
                        {
                            ParameterName = "@vchMensaje",
                            Value = "",
                            Direction = ParameterDirection.Output,
                            SqlDbType = SqlDbType.VarChar,
                            Size = 255
                        };
                        var returnCode = new SqlParameter
                        {
                            ParameterName = "@returnCode",
                            Value = -1,
                            Direction = ParameterDirection.Output
                        };

                        await potigianContext.Database.ExecuteSqlCommandAsync("EXEC @returnCode = dbo.SD03_OC_ING_MERC_ACTU_DETA " +
                            "@C_OC, @C_SUCU_EMPR, @C_SUCU_DESTINO_ALT, @U_PREFIJO_OC, @U_SUFIJO_OC, @M_OC_MADRE, @C_SITUAC_OC, " +
                            "@C_ARTICULO, @M_VENDE_POR_PESO, @Q_UNID_KGS_PED, @Q_UNID_KGS_CUMPL, @Q_UNID_KGS_PEND, " +
                            "@Q_UNID_KGS_ING, @Q_BULTOS_KGS_ING, @Q_FACTOR_PIEZAS_ING, @C_COMP_ING_MERC, " +
                            "@U_PREFIJO_COMP_ING_MERC, @U_SUFIJO_COMP_ING_MERC, @F_COMP_ING_MERC, @C_PROGRAMA, @D_OBSERVACION, " +
                            "@vchUsuario, @vchTerminal, @vchMensaje OUTPUT", returnCode, ocParam, companyBranchParam, destinationBranchParam,
                            prefixOcParam, suffixOcParam, ocMotherParam, situationCodeParam, articleCodeParam,
                            saleByWeightParam, requestedUnitsParam, deliveredUnitsParam, pendingUnitsParam, receivedUnitsParam,
                            receivedPacksParam, receivedPiecesFactorParam, receivedGoodsBoughtParam, receivedGoodsPrefixParam,
                            receivedGoodsSuffixParam, goodsDateParam, programCodeParam, observationParam, userParam,
                            terminalParam, outputMessageParam);

                        response.DetailInfo.Add(new SynchronizeOrderDetail
                        {
                            ArticleCode = orderDetail.ArticleCode,
                            DetailMessage = (string)outputMessageParam.Value,
                            DetailReturnCode = (int)returnCode.Value
                        });
                    }

                    potigianContext.Database.CommitTransaction();

                    return response;
                });
        }
    }
}
