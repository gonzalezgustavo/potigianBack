using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<ActionResult<Response<PurchaseOrder>>> GetOrderById(int prefixOc, int oc, int suffixOc)
        {
            return await RequestsHandler.HandleAsyncRequest(
                async () => await potigianContext.PurchaseOrders.Where(o => o.OrderPrefix == prefixOc && o.OrderCode == oc && o.OrderSuffix == suffixOc && o.Situation == 1).FirstOrDefaultAsync());
        }

        [HttpGet("proveedor/{code}")]
        public async Task<ActionResult<Response<List<PurchaseOrder>>>> GetOrdersByProvider(int code)
        {
            return await RequestsHandler.HandleAsyncRequest(
                async () => await potigianContext.PurchaseOrders.Where(o => o.ProviderCode == code && o.Situation == 1).ToListAsync());
        }
    }
}
