using System;
using System.Collections.Generic;
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

        [HttpGet("{ocCode}/proveedor/{providerCode}")]
        public async Task<ActionResult<Response<List<PurchaseOrderHeader>>>> GetOrdersByProviderAndOC(string providerCode, string ocCode)
        {
            return await RequestsHandler.HandleAsyncRequest(
                async () =>
                {
                    var expr = default(Expression<Func<PurchaseOrderHeader, bool>>);
                    if (providerCode == "none" && ocCode == "none")
                    {
                        expr = o => o.Situation == 1;
                    }
                    else if (providerCode == "none")
                    {
                        expr = o => o.OrderCode.ToString().Contains(ocCode) && o.Situation == 1;
                    }
                    else if (ocCode == "none")
                    {
                        expr = o => o.ProviderCode.ToString().Contains(providerCode) && o.Situation == 1;
                    }
                    else
                    {
                        expr = o => o.OrderCode.ToString().Contains(ocCode) && o.ProviderCode.ToString().Contains(providerCode) && o.Situation == 1;
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
                    .ToListAsync());
        }
    }
}
