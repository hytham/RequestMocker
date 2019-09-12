using System;
using System.Collections.Generic;
using System.Net.Http;

namespace RequestMocker
{
    public class RequestMockerOptions
    {
        public Dictionary<string, Tuple<HttpMethod, object>> MockedRouteingTable { get; private set; }
        public RequestMockerOptions()
        {
            MockedRouteingTable = new Dictionary<string, Tuple<HttpMethod, object>>();
        }
        /// <summary>
        /// Map the giving route regular expression and method to the expected response
        /// </summary>
        /// <param name="method">The request Method</param>
        /// <param name="route">The request route</param>
        /// <param name="response">The response if the mapping is success</param>
        public void Map(HttpMethod method, string route, object response)
        {
            MockedRouteingTable.Add(route.ToLower(), Tuple.Create(method, response));
        }


    }
}
