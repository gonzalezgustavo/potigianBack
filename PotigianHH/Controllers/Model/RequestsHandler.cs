using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace PotigianHH.Controllers.Model
{
    public static class RequestsHandler
    {
        public static async Task<ActionResult<Response<T>>> HandleAsyncRequest<T>(ILogger logger, Func<Task<T>> lambda)
        {
            var response = new Response<T>();

            try
            {
                response.Object = await lambda();
                response.Success = true;
            }
            catch (Exception ex)
            {
                logger.LogError("Excepción: " + ex.ToString());
                response.Success = false;
                response.ErrorInfo = new ErrorInfo
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };
            }

            return response;
        }

        public static ActionResult<Response<T>> HandleRequest<T>(ILogger logger, Func<T> lambda)
        {
            var response = new Response<T>();

            try
            {
                response.Object = lambda();
                response.Success = true;
            }
            catch (Exception ex)
            {
                logger.LogError("Excepción: " + ex.ToString());
                response.Success = false;
                response.ErrorInfo = new ErrorInfo
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };
            }

            return response;
        }
    }
}
