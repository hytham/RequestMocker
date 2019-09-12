using System;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using RequestMocker;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net.Http;
using FluentAssertions;
using System.IO;

namespace RequestMockerUnitTest
{
    public class RequestMockerTest
    {
        [Theory]
        [InlineData("/sample", "Get")]
        [InlineData("/sample/*","Get")]
        [InlineData("/sample/*/test1", "Get")]       
        public void TestRequestForstaticRout(string route, string method)
        {

            Dictionary<string, Tuple<HttpMethod, object>> mockedRouteingTable = new Dictionary<string, Tuple<HttpMethod, object>>
            {
                {@"\/sample(?![\w\d])",new Tuple<HttpMethod, object>(HttpMethod.Get,new {test =1 }) }
            };
            string request = "{id:1}";
            var context = new DefaultHttpContext();
            context.Request.Method = method;
            context.Request.Path = new PathString(route);
            context.Request.Body = new MemoryStream();
            context.Request.Body.Write(System.Text.ASCIIEncoding.ASCII.GetBytes(request), 0, request.Length);
            context.Request.Body.Flush();
            var mocker = RequestMocker.RequestMocker.TestRequest(context, mockedRouteingTable);
            mocker.Should().Contain("1", mocker);
        }

        [Theory]      
        [InlineData("/sample1", "Get")]
        public void TestRequestForstaticRoutInvalid(string route, string method)
        {

            Dictionary<string, Tuple<HttpMethod, object>> mockedRouteingTable = new Dictionary<string, Tuple<HttpMethod, object>>
            {
                {@"\/sample(?![\w\d])",new Tuple<HttpMethod, object>(HttpMethod.Get,new {test =1 }) }
            };
            string request = "{id:1}";
            var context = new DefaultHttpContext();
            context.Request.Method = method;
            context.Request.Path = new PathString(route);
            context.Request.Body = new MemoryStream();
            context.Request.Body.Write(System.Text.ASCIIEncoding.ASCII.GetBytes(request), 0, request.Length);
            context.Request.Body.Flush();
            var mocker = RequestMocker.RequestMocker.TestRequest(context, mockedRouteingTable);
            mocker.Should().BeEmpty(mocker);
        }
    }
}
