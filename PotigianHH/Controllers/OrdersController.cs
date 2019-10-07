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

        [HttpGet("{prefixOc}/{oc}/{suffixOc}")]
        public async Task<ActionResult<Response<PurchaseOrderHeader>>> GetOrderById(int prefixOc, int oc, int suffixOc)
        {
            return await RequestsHandler.HandleAsyncRequest(
                async () => await potigianContext.PurchaseOrdersHeaders.Where(o => o.OrderPrefix == prefixOc && o.OrderCode == oc && o.OrderSuffix == suffixOc && o.Situation == 1).FirstOrDefaultAsync());
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

                    return await potigianContext.PurchaseOrdersHeaders.Where(expr).ToListAsync();
                });
        }
    }
}
