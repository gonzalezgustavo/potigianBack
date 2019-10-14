using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration config;

        public RequestsController(PotigianContext potigianContext, IConfiguration config)
        {
            this.potigianContext = potigianContext;
            this.config = config;
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
                .Where(h => (h.PreparerCode == preparer.ToString() || h.PreparerCode == $"CSPRP{preparer}") && h.SituationCode == Config.Requests.StateInPreparation)
                .ToListAsync());
        }

        [HttpPost("cabe/asignados/{preparer}")]
        public async Task<ActionResult<Response<List<RequestHeaders>>>> AssignRequestsHeadersToPreparer(int preparer, [FromQuery(Name = "cigarrillos")] bool cigarettesOnly)
        {
            return await RequestsHandler.HandleAsyncRequest(
                async () =>
                {
                    var assignedRequests = await potigianContext.RequestHeaders
                    .Where(req =>
                        (req.PreparerCode == preparer.ToString() || req.PreparerCode == $"CSPRP{preparer}") && req.SituationCode == Config.Requests.StateInPreparation)
                    .ToListAsync();

                    // If there are any assigned requests, we won't assign new ones.
                    if (assignedRequests.Count > 0)
                    {
                        return assignedRequests;
                    }

                    var newRequests = await potigianContext.RequestHeaders
                        .Where(req => req.SituationCode == Config.Requests.StateAvailableToPrepare)
                        .ToListAsync();

                    var suffixes = newRequests.Select(req => req.DocumentSuffix).Distinct();
                    var prefixes = newRequests.Select(req => req.DocumentPrefix).Distinct();
                    var docs = newRequests.Select(req => req.DocumentCode).Distinct();

                    var requestsDetails = await potigianContext.RequestDetails
                        .Where(req => suffixes.Contains(req.DocumentSuffix) &&
                                      prefixes.Contains(req.DocumentPrefix) &&
                                      docs.Contains(req.DocumentCode))
                        .ToListAsync();

                    var definedRequests = newRequests.Where(req =>
                        {
                            var details = requestsDetails.Where(det => req.DocumentSuffix == det.DocumentSuffix
                                                                     && req.DocumentCode == det.DocumentCode
                                                                     && req.DocumentPrefix == det.DocumentPrefix);
                            bool areOnlyCigarettes = details.All(det => new List<decimal?>() { 1, 2 }.Contains(det.FamilyCode));

                            return cigarettesOnly ? areOnlyCigarettes : !areOnlyCigarettes;
                        })
                        .Take(config.GetValue<int>("RequestsPerPreparer"))
                        .ToList();

                    definedRequests.ForEach(req =>
                    {
                        req.SituationCode = Config.Requests.StateInPreparation;
                        req.SituationDate = DateTime.Now;
                        req.PreparerCode = preparer.ToString();

                        potigianContext.Update(req);
                    });

                    await potigianContext.SaveChangesAsync();

                    return definedRequests;
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
                async () =>
                {
                    var requestDetails = await potigianContext.RequestDetails
                        .Where(req =>
                            req.DocumentPrefix == prefixDoc &&
                            req.DocumentCode == doc &&
                            req.DocumentSuffix == suffixDoc)
                        .Join(
                            potigianContext.Articles,
                            req => req.ArticleCode,
                            article => article.Code,
                            (req, article) => req.Append(article))
                        .ToListAsync();

                    foreach (var req in requestDetails)
                    {
                        var value = await potigianContext.BranchArticles.FirstOrDefaultAsync(a => a.ArticleCode == req.ArticleCode);
                        if (value.ArticleCode == null || value.ArticleCode == 0)
                        {
                            value.ArticleCode = 1;
                        }
                        req.Append(value);
                    }

                    return requestDetails;
                });

        }

        [HttpGet("preparaciones")]
        public async Task<ActionResult<Response<List<RequestPreparation>>>> GetRequestsPreparations()
        {
            return await RequestsHandler.HandleAsyncRequest(
                async () => await potigianContext.RequestPreparations.ToListAsync());
        }

        [HttpPost("preparaciones/{prefixDoc}/{doc}/{suffixDoc}")]
        public async Task<ActionResult<Response<bool>>> CloseRequestPreparation(int prefixDoc, int doc, int suffixDoc, [FromBody] IDictionary<decimal?, int> articleCount)
        {
            return await RequestsHandler.HandleAsyncRequest(
                async () =>
                {
                    var requestDetails = await potigianContext.RequestDetails
                        .Where(req =>
                            req.DocumentPrefix == prefixDoc &&
                            req.DocumentCode == doc &&
                            req.DocumentSuffix == suffixDoc)
                        .ToListAsync();
                    var unfinishedRequestDetails = requestDetails
                        .Where(req => req.RequestItem != articleCount[req.ArticleCode]);
                    var finishedRequestDetails = requestDetails
                        .Where(req => req.RequestItem == articleCount[req.ArticleCode]);
                    var requestHeader = await potigianContext.RequestHeaders
                        .FirstAsync(req => req.DocumentPrefix == prefixDoc && req.DocumentCode == doc && req.DocumentSuffix == suffixDoc);

                    // Unfinished requests case
                    if (unfinishedRequestDetails.Count() > 0)
                    {
                        var missingRequestDetails = unfinishedRequestDetails
                            .Select(req => new RequestMissingDetails(req, articleCount[req.ArticleCode]))
                            .ToList();
                        var requestsToRemove = requestDetails.Where(reqDetail => !finishedRequestDetails.Any(
                            fReq => reqDetail.DocumentCode == fReq.DocumentCode &&
                                      reqDetail.DocumentPrefix == fReq.DocumentPrefix &&
                                      reqDetail.DocumentSuffix == fReq.DocumentSuffix &&
                                      reqDetail.ArticleCode == fReq.ArticleCode));

                        var requestsToUpdate = requestDetails.Where(reqDetail => !unfinishedRequestDetails.Any(
                            unfReq => reqDetail.DocumentCode == unfReq.DocumentCode &&
                                      reqDetail.DocumentPrefix == unfReq.DocumentPrefix &&
                                      reqDetail.DocumentSuffix == unfReq.DocumentSuffix &&
                                      reqDetail.ArticleCode == unfReq.ArticleCode));

                        foreach (var request in requestsToUpdate)
                        {
                            request.PackagesGrams = request.PackagesGrams - articleCount[request.ArticleCode];
                            request.ArticleTotal = request.PackagesGrams * request.FinalArticleUnitaryPrice;
                        }

                        potigianContext.RequestMissingDetails.AddRange(missingRequestDetails);
                        potigianContext.RequestDetails.RemoveRange(requestsToRemove);
                        potigianContext.RequestDetails.UpdateRange(requestsToUpdate);

                        await potigianContext.SaveChangesAsync();

                        return false;
                    }

                    // Success!
                    var preparation = new RequestPreparation
                    {
                        // TBD
                        // MovementFlag = ?
                        Code = requestHeader.PreparerCode.ToString(),
                        DocumentSuffix = suffixDoc,
                        InsertDate = DateTime.Now,
                        StartDate = requestHeader.SituationDate,
                        StatusCode = Config.Requests.StateClosed,
                        EndDate = DateTime.Now,
                    };

                    requestHeader.SituationCode = Config.Requests.StateClosed;
                    requestHeader.SituationDate = DateTime.Now;
                    potigianContext.Update(requestHeader);

                    potigianContext.RequestPreparations.Add(preparation);

                    await potigianContext.SaveChangesAsync();

                    return true;
                });
        }
    }
}
