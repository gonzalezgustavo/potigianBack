using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PotigianHH.Controllers.Model;
using PotigianHH.Database;
using PotigianHH.Model;

namespace PotigianHH.Controllers
{
    [Route("api/preparadores")]
    [ApiController]
    public class PreparersController : ControllerBase
    {
        private readonly PotigianContext potigianContext;

        public PreparersController(PotigianContext potigianContext)
        {
            this.potigianContext = potigianContext;
        }

        

        [HttpGet]
        public async Task<ActionResult<Response<List<Preparer>>>> GetPreparers()
        {
            return await RequestsHandler.HandleAsyncRequest(
                async () => await potigianContext.Preparers.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Response<Preparer>>> GetPreparer(int id)
        {
            return await RequestsHandler.HandleAsyncRequest(
                async () => 
                {
                    var preparer = await potigianContext.Preparers.FirstOrDefaultAsync(p => p.Id == id);

                    if (preparer == null)
                    {
                        preparer = new Preparer
                        {
                            Code = "-1"
                        };
                    }

                    return preparer;
                });
        }

    }
}
