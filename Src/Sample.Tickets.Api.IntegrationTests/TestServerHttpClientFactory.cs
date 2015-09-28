using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.Owin.Testing;
using Sample.Api.Shared;

namespace Sample.Tickets.Api.IntegrationTests
{
    /// <summary>
    /// Uses OWIN test server to host api under test and creats http client for that server
    /// </summary>
    public static class TestServerHttpClientFactory
    {
        /// <summary>
        /// Creates test server http client which can be used for api integration tests
        /// </summary>
        public static HttpClient Create(string userName)
        {
            var server = TestServer.Create(app => new OwinStartup().Configuration(app));
            var client = server.HttpClient;
            client.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", new SimpleToken(new Claim("userName", userName)).ToString());
            return client;
        }
    }
}
