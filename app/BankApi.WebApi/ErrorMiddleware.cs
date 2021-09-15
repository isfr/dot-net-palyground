using System;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;

using BankApi.Domain.Exceptions;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BankApi.WebApi
{
    public class ErrorMiddleware
    {
        private readonly RequestDelegate next;

        private readonly ILogger logger;

        public ErrorMiddleware(RequestDelegate next, ILogger<ErrorMiddleware> logger)
        {
            this.next = next;

            this.logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await this.next.Invoke(httpContext);
            }
            catch (BusinessException bEx)
            {
                if (httpContext.Response.HasStarted)
                {
                    this.logger.LogError("Response Started!. Error middleware can not set Status Code!");
                    throw;
                }

                httpContext.Response.Clear();
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                httpContext.Response.ContentType = MediaTypeNames.Application.Json;

                var error = new BaseError(bEx);

                var response = Newtonsoft.Json.JsonConvert.SerializeObject(error);

                await httpContext.Response.WriteAsync(response);
                logger.LogError(bEx, response);
            }
            catch (Exception ex)
            {
                if (httpContext.Response.HasStarted)
                {
                    this.logger.LogError("Response Started!. Error middleware can not set Status Code!");
                    throw;
                }

                httpContext.Response.Clear();
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                httpContext.Response.ContentType = MediaTypeNames.Application.Json;

                var error = new BaseError(ex);

                var response = Newtonsoft.Json.JsonConvert.SerializeObject(error);

                await httpContext.Response.WriteAsync(response);
                logger.LogError(ex, response);
            }
        }
    }

    public static class ErrorMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorMidleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorMiddleware>();
        }
    }
}
