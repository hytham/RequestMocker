using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RequestMocker
{
    public class RequestMocker
    {
        private readonly RequestDelegate _next;
        private readonly RequestMockerOptions _options;

        public RequestMocker(RequestDelegate next, IOptions<RequestMockerOptions> options)
        {
            _next = next;
            _options = options.Value;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var matchedRequest = TestRequest(httpContext, _options.mockedRouteingTable);
            if (string.IsNullOrEmpty(matchedRequest))
            {
                await _next.Invoke(httpContext);
            }
            else
            {
                await httpContext.Response.WriteAsync(matchedRequest);
            }
        }

        private string TestRequest(HttpContext context, Dictionary<string, Tuple<HttpMethod, object>> mockedRouteingTable)
        {

            var route = mockedRouteingTable.Keys.FirstOrDefault(x => x.Equals(context.Request.Path.Value.Trim().ToLower()));

            if (route != null)
            {
                var responeBody = mockedRouteingTable[route];
                context.Response.StatusCode = 200;
                if (responeBody.Item1.ToString().ToLower() == context.Request.Method.ToLower())
                {
                    var responseString = JsonConvert.SerializeObject(responeBody.Item2);
                    return responseString;
                }
                return string.Empty;

            }
            else return string.Empty;

        }
    }
    public static class RequestMockerExtensions
    {
        public static IServiceCollection AddRequestMocker(this IServiceCollection service, Action<RequestMockerOptions> options)
        {
            options = options ?? (opts => { });
            service.Configure(options);
            return service;
        }
        public static IApplicationBuilder UseRequestMocker(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestMocker>();
        }
    }
}
