﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PotigianHH.Controllers.Model;
using PotigianHH.Database;
using PotigianHH.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PotigianHH.Controllers
{
    [Route("api/pedidos")]
    [ApiController]
    public class RequestsController : ControllerBase
    {
        private readonly PotigianContext potigianContext;
        private readonly IConfiguration config;
        private readonly ILogger<RequestsController> logger;

        public RequestsController(PotigianContext potigianContext, IConfiguration config, ILogger<RequestsController> logger)
        {
            this.potigianContext = potigianContext;
            this.config = config;
            this.logger = logger;
        }

        [HttpGet("cabe")]
        public async Task<ActionResult<Response<List<RequestHeaders>>>> GetRequestsHeaders()
        {
            logger.LogInformation("GET cabe invocado");
            return await RequestsHandler.HandleAsyncRequest(
                logger,
                async () => await potigianContext.RequestHeaders
                    .ToListAsync());
        }

        [HttpGet("cabe/situacion/{situation}")]
        public async Task<ActionResult<Response<List<RequestHeaders>>>> GetRequestsHeadersBySituation(int situation)
        {
            logger.LogInformation("GET cabe/situacion/{code} invocado con code " + situation);
            return await RequestsHandler.HandleAsyncRequest(
                logger,
                async () => await potigianContext.RequestHeaders
                    .Where(h => h.SituationCode == situation)
                    .ToListAsync());
        }

        [HttpGet("cabe/asignados/{preparer}")]
        public async Task<ActionResult<Response<List<RequestHeaders>>>> GetRequestsHeadersByPreparer(int preparer)
        {
            logger.LogInformation("GET cabe/asignados/{preparador} invocado con preparador " + preparer);
            return await RequestsHandler.HandleAsyncRequest(
                logger,
                async () => await potigianContext.RequestHeaders
                .Where(h => (h.PreparerCode == preparer.ToString() || h.PreparerCode == $"CSPRP{preparer}") && h.SituationCode == Config.Requests.StateInPreparation)
                .ToListAsync());
        }

        [HttpPost("cabe/asignados/{prefixDoc}/{doc}/{suffixDoc}/start/{preparer}")]
        public async Task<ActionResult<Response<bool>>> StartRequest(int prefixDoc, int doc, int suffixDoc, int preparer)
        {
            logger.LogInformation("POST cabe/asignados/{prefijo}/{doc}/{sufijo}/start/{preparador} invocado con prefijo " + prefixDoc + ", doc " + doc + ", sufijo " + suffixDoc + " y preparador " + preparer);
            return await RequestsHandler.HandleAsyncRequest(
                logger,
                async () =>
                {
                    var requestPreparations = potigianContext.RequestPreparations.Where(rp => rp.DocumentSuffix == suffixDoc);
                    if (await requestPreparations.CountAsync() == 0) // new entry
                    {
                        var preparation = new RequestPreparation
                        {
                            Code = preparer.ToString(),
                            DocumentSuffix = suffixDoc,
                            InsertDate = DateTime.Now,
                            StartDate = DateTime.Now,
                            StatusCode = Config.Requests.StateInPreparation,
                            EndDate = null,
                        };

                        potigianContext.Add(preparation);
                        potigianContext.Add(PreparerProductivity.FromPreparation(preparation));

                        await potigianContext.SaveChangesAsync();
                    } 
                    else
                    {
                        try
                        {
                            var preparation = await requestPreparations.FirstAsync();

                            if (preparation.StartDate == null)
                            {
                                preparation.Code = preparer.ToString();
                                preparation.StartDate = DateTime.Now;
                                preparation.EndDate = null;
                                preparation.StatusCode = Config.Requests.StateInPreparation;

                                potigianContext.Update(preparation);
                                potigianContext.Add(PreparerProductivity.FromPreparation(preparation));

                                await potigianContext.SaveChangesAsync();
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }

                    return true;
                });
        }

        [HttpPost("cabe/asignados/{preparer}/clear")]
        public async Task<ActionResult<Response<bool>>> ClearAssignedRequestsFromPreparer(int preparer)
        {
            logger.LogInformation("POST cabe/asignados/{preparador}/clear invocado con preparador " + preparer);
            return await RequestsHandler.HandleAsyncRequest(
                logger,
                async () =>
                {
                    var assignedRequests = await potigianContext.RequestHeaders
                    .Where(req =>
                        (req.PreparerCode == preparer.ToString() || req.PreparerCode == $"CSPRP{preparer}") && req.SituationCode == Config.Requests.StateInPreparation)
                    .ToListAsync();

                    assignedRequests.ForEach(r =>
                    {
                        r.SituationCode = Config.Requests.StateAvailableToPrepare;
                        r.SituationDate = DateTime.Now;
                        r.PreparerCode = null;

                        potigianContext.Update(r);
                    });

                    await potigianContext.SaveChangesAsync();

                    return true;
                });
        }

        [HttpPost("cabe/asignados/{preparer}")]
        public async Task<ActionResult<Response<List<RequestHeaders>>>> AssignRequestsHeadersToPreparer(
            int preparer, 
            [FromQuery(Name = "cigarrillos")] bool cigarettesOnly,
            [FromQuery(Name = "pedido")] int suffixNumber,
            [FromQuery(Name = "reparto")] int deliveryNumber)
        {
            logger.LogInformation("POST cabe/asignados/{preparador} invocado con preparador " + preparer);
            return await RequestsHandler.HandleAsyncRequest(
                logger,
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

                    var newRequests = default(List<RequestHeaders>);

                    if (suffixNumber != 0)
                    {
                        newRequests = await potigianContext.RequestHeaders
                            .Where(req => req.DocumentSuffix == suffixNumber && req.SituationCode == Config.Requests.StateAvailableToPrepare)
                            .ToListAsync();
                    }
                    else if (deliveryNumber != 0)
                    {
                        newRequests = await potigianContext.RequestHeaders
                            .Where(req => req.DistributionNumber == deliveryNumber && req.SituationCode == Config.Requests.StateAvailableToPrepare)
                            .ToListAsync();
                    }

                    if (newRequests == default(List<RequestHeaders>) || newRequests.Count == 0)
                    {
                        return new List<RequestHeaders>();
                    }

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
                            var details = requestsDetails
                                .Where(det => req.DocumentSuffix == det.DocumentSuffix
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
            logger.LogInformation("GET detalle invocado");
            return await RequestsHandler.HandleAsyncRequest(
                logger,
                async () => await potigianContext.RequestDetails.ToListAsync());
        }

        [HttpGet("detalle/{prefixDoc}/{doc}/{suffixDoc}")]
        public async Task<ActionResult<Response<List<RequestDetails>>>> GetRequestDetails(int prefixDoc, int doc, int suffixDoc)
        {
            logger.LogInformation("GET detalle/{prefijo}/{doc}/{sufijo} invocado con prefijo " + prefixDoc + ", doc " + doc + " y sufijo " + suffixDoc);
            return await RequestsHandler.HandleAsyncRequest(
                logger,
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
            logger.LogInformation("GET preparaciones invocado");
            return await RequestsHandler.HandleAsyncRequest(
                logger,
                async () => await potigianContext.RequestPreparations.ToListAsync());
        }

        [HttpPost("preparaciones/{prefixDoc}/{doc}/{suffixDoc}")]
        public async Task<ActionResult<Response<bool>>> CloseRequestPreparation(int prefixDoc, int doc, int suffixDoc, [FromBody] CloseRequestPayload payload)
        {
            logger.LogInformation("POST preparaciones/{prefijo}/{doc}/{sufijo} invocado con prefijo " + prefixDoc + ", doc " + doc + " y sufijo " + suffixDoc);
            return await RequestsHandler.HandleAsyncRequest(
                logger,
                async () =>
                {
                    bool closedComplete = true;

                    var requestDetails = await potigianContext.RequestDetails
                        .Where(req =>
                            req.DocumentPrefix == prefixDoc &&
                            req.DocumentCode == doc &&
                            req.DocumentSuffix == suffixDoc)
                        .ToListAsync();
                    var unfinishedRequestDetails = requestDetails
                        .Where(req => req.PackagesGrams != payload.ArticleCount[GetDictionaryCountKey(req)]);
                    var requestHeader = await potigianContext.RequestHeaders
                        .FirstAsync(req => req.DocumentPrefix == prefixDoc && req.DocumentCode == doc && req.DocumentSuffix == suffixDoc);

                    // Unfinished requests case
                    if (unfinishedRequestDetails.Count() > 0)
                    {
                        var missingRequestDetails = unfinishedRequestDetails
                            .Select(req => new RequestMissingDetails(req, payload.ArticleCount[GetDictionaryCountKey(req)]))
                            .ToList();

                        foreach (var request in unfinishedRequestDetails)
                        {
                            int count = payload.ArticleCount[GetDictionaryCountKey(request)];

                            if (count != 0)
                            {
                                request.PackagesGrams = count;
                                request.ArticleTotal = request.PackagesGrams * request.ArticleUnitaryPrice;
                                potigianContext.RequestDetails.Update(request);
                            }
                        }

                        potigianContext.RequestMissingDetails.AddRange(missingRequestDetails);
                        closedComplete = false;
                    }

                    var preparation = await potigianContext.RequestPreparations.FirstAsync(p => p.DocumentSuffix == suffixDoc);
                    preparation.EndDate = DateTime.Now;
                    preparation.StatusCode = Config.Requests.StateClosed;
                    preparation.MovementFlag = 'E';

                    var productivity = await potigianContext.PreparerProductivities.FirstAsync(p => p.DocumentSuffix == suffixDoc);
                    productivity.EndDate = preparation.EndDate;
                    productivity.StatusCode = preparation.StatusCode;
                    productivity.MovementFlag = preparation.MovementFlag;

                    var productivityDifferential = productivity.EndDate.Value.Subtract(productivity.StartDate.Value);
                    productivity.Days = productivityDifferential.Days;
                    productivity.Hours = productivityDifferential.Hours;
                    productivity.Minutes = productivityDifferential.Minutes;
                    productivity.Seconds = productivityDifferential.Seconds;

                    requestHeader.SituationCode = Config.Requests.StateClosed;
                    requestHeader.SituationDate = DateTime.Now;
                    requestHeader.Printer = payload.Printer;
                    requestHeader.TotalPackages = payload.Bags;

                    potigianContext.RequestHeaders.Update(requestHeader);
                    potigianContext.RequestPreparations.Update(preparation);
                    potigianContext.PreparerProductivities.Update(productivity);

                    await potigianContext.SaveChangesAsync();

                    return closedComplete;
                });
        }

        private string GetDictionaryCountKey(RequestDetails request)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Key format: '#1000-1#' - Used to have a single string key on the dictionary
                string toFormat = "#" + request.ArticleCode + "-" + request.RequestItem.Value + "#";
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(toFormat));
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    sb.Append(bytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        } 
    }
}
