using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RequestMocker
{
    /// <summary>
    /// This is the main class
    /// </summary>
    public class RequestMocker
    {
        /// <summary>
        /// The next middle-ware
        /// </summary>
        private readonly RequestDelegate _next;
        /// <summary>
        /// The request Mocker Options
        /// </summary>
        private readonly RequestMockerOptions _options;

        /// <see cref="RequestMocker"/>
        /// <summary>
        /// The middle-ware constructor
        /// </summary>
        /// <param name="next">The next unit down the pipeline</param>
        /// 
        /// <param name="options">The Request Mocker Options</param>
        /// 
        public RequestMocker(RequestDelegate next, IOptions<RequestMockerOptions> options)
        {
            _next = next;
            _options = options.Value;
        }

        /// <summary>
        /// This will invoke Middle-ware
        /// </summary>
        /// <param name="httpContext">The HTTP Context that is passed by the Dot Net Core</param>
        /// <returns>The invoked Task</returns>
        public async Task Invoke(HttpContext httpContext)
        {
            var matchedRequest = TestRequest(httpContext, _options.mockedRouteingTable);
            if (string.IsNullOrEmpty(matchedRequest))
            {
                await this._next.Invoke(httpContext);
            }
            else
            {
                await httpContext.Response.WriteAsync(matchedRequest);
            }
        }

        /// <summary>
        /// This will test the incoming request to match the route in the routing table
        /// </summary>
        /// <param name="context">The HTTP Context</param>
        /// <param name="mockedRouteingTable">The routing table</param>
        /// <returns>The response string</returns>
        public static string TestRequest(HttpContext context, Dictionary<string, Tuple<HttpMethod, object>> mockedRouteingTable)
        {

            var route = mockedRouteingTable.Keys.FirstOrDefault(x => Regex.IsMatch(context.Request.Path.Value.Trim().ToLower(), x));

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
        /// <summary>
        /// Setup the RequestMocker Service
        /// </summary>
        /// <param name="service">The Service Collection</param>
        /// <param name="options">Request Mocker Options</param>
        /// <returns>The service Collection Object</returns>
        public static IServiceCollection AddRequestMocker(this IServiceCollection service, Action<RequestMockerOptions> options)
        {
            options = options ?? (opts => { });
            service.Configure(options);
            return service;
        }
        /// <summary>
        /// Add the Request Mocker to the DotNetCore Pipeline
        /// </summary>
        /// 
        /// <param name="builder">The application builder</param>
        /// 
        /// <returns>the application builder</returns>
        /// 
        public static IApplicationBuilder UseRequestMocker(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestMocker>();
        }
    }
}
