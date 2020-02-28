using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<PreparersController> logger;

        public PreparersController(PotigianContext potigianContext, ILogger<PreparersController> logger)
        {
            this.potigianContext = potigianContext;
            this.logger = logger;
        }       

        [HttpGet]
        public async Task<ActionResult<Response<List<Preparer>>>> GetPreparers()
        {
            logger.LogInformation("GET preparadores invocado");
            return await RequestsHandler.HandleAsyncRequest(
                logger,
                async () => await potigianContext.Preparers.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Response<Preparer>>> GetPreparer(int id)
        {
            logger.LogInformation("GET preparadores/{id} invocado con id " + id);
            return await RequestsHandler.HandleAsyncRequest(
                logger,
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
