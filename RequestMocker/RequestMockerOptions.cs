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
    public class RequestMockerOptions
    {
        public Dictionary<string, Tuple<HttpMethod, object>> mockedRouteingTable { get; private set; }
        public RequestMockerOptions()
        {
            mockedRouteingTable = new Dictionary<string, Tuple<HttpMethod, object>>();
        }
        /// <summary>
        /// Map the giving route regular expression and method to the expected response
        /// </summary>
        /// <param name="method">The request Method</param>
        /// <param name="route">The request route</param>
        /// <param name="response">The response if the mapping is success</param>
        public void Map(HttpMethod method, string route, object response)
        {
            mockedRouteingTable.Add(route.ToLower(), Tuple.Create(method, response));
        }

       
    }
}
