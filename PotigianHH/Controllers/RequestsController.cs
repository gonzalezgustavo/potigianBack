using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PotigianHH.Controllers.Model;
using PotigianHH.Database;
using PotigianHH.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PotigianHH.Controllers
{
    [Route("api/pedidos")]
    [ApiController]
    public class RequestsController : ControllerBase
    {
        private readonly PotigianContext potigianContext;

        public RequestsController(PotigianContext potigianContext)
        {
            this.potigianContext = potigianContext;
        }

        [HttpGet("cabe")]
        public async Task<ActionResult<Response<List<RequestHeaders>>>> GetRequestsHeaders()
        {
            return await RequestsHandler.HandleAsyncRequest(
                async () => await potigianContext.RequestHeaders
                    .ToListAsync());
        }

        [HttpGet("cabe/situacion/{situation}")]
        public async Task<ActionResult<Response<List<RequestHeaders>>>> GetRequestsHeadersBySituation(int situation)
        {
            return await RequestsHandler.HandleAsyncRequest(
                async () => await potigianContext.RequestHeaders
                    .Where(h => h.SituationCode == situation)
                    .ToListAsync());
        }

        [HttpGet("cabe/asignados/{preparer}")]
        public async Task<ActionResult<Response<List<RequestHeaders>>>> GetRequestsHeadersByPreparer(int preparer)
        {
            return await RequestsHandler.HandleAsyncRequest(
                async () => await potigianContext.RequestHeaders
                .Where(h => h.PreparerCode == preparer && h.SituationCode == Config.Requests.StateInPreparation)
                .ToListAsync());
        }

        [HttpPost("cabe/asignados/{preparer}")]
        public async Task<ActionResult<Response<List<RequestHeaders>>>> AssignRequestsHeadersToPreparer(int preparer, [FromQuery] bool cigarettesOnly)
        {
            return await RequestsHandler.HandleAsyncRequest(
                async () =>
                {
                    var assignedRequests = await potigianContext.RequestHeaders
                    .Where(req =>
                        req.PreparerCode == preparer && req.SituationCode == Config.Requests.StateInPreparation)
                    .ToListAsync();

                    // If there are any assigned requests, we won't assign new ones.
                    if (assignedRequests.Count > 0)
                    {
                        return assignedRequests;
                    }

                    var newRequests = await potigianContext.RequestHeaders
                        .Where(req => req.SituationCode == Config.Requests.StateAvailableToPrepare)
                        .Take(Config.Requests.RequestsPerPreparer)
                        .ToListAsync();

                    newRequests.ForEach(req =>
                    {
                        req.SituationCode = Config.Requests.StateInPreparation;
                        req.SituationDate = DateTime.Now;
                        req.PreparerCode = preparer;

                        potigianContext.Update(req);
                    });

                    await potigianContext.SaveChangesAsync();

                    return newRequests;
                });
        }

        [HttpGet("detalle")]
        public async Task<ActionResult<Response<List<RequestDetails>>>> GetRequestsDetails()
        {
            return await RequestsHandler.HandleAsyncRequest(
                async () => await potigianContext.RequestDetails.ToListAsync());
        }

        [HttpGet("detalle/{prefixDoc}/{doc}/{suffixDoc}")]
        public async Task<ActionResult<Response<List<RequestDetails>>>> GetRequestDetails(int prefixDoc, int doc, int suffixDoc)
        {
            return await RequestsHandler.HandleAsyncRequest(
                async () => await potigianContext.RequestDetails
                    .Where(req =>
                        req.DocumentPrefix == prefixDoc &&
                        req.DocumentCode == doc &&
                        req.DocumentSuffix == suffixDoc)
                    .Join(
                        potigianContext.Articles,
                        req => req.ArticleCode,
                        article => article.Code,
                        (req, article) => req.Append(article))
                    .ToListAsync()); ;
        }

        [HttpGet("preparaciones")]
        public async Task<ActionResult<Response<List<RequestPreparation>>>> GetRequestsPreparations()
        {
            return await RequestsHandler.HandleAsyncRequest(
                async () => await potigianContext.RequestPreparations.ToListAsync());
        }

        // TODO: preparar el finalizado de los pedidos
    }
}
