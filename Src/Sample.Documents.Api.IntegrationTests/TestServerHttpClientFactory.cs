using Microsoft.Owin.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Documents.Api.IntegrationTests
{
    /// <summary>
    /// Uses OWIN test server to host api under test and creats http client for that server
    /// </summary>
    public static class TestServerHttpClientFactory
    {
        /// <summary>
        /// Creates test server http client which can be used for api integration tests
        /// </summary>
        public static HttpClient Create()
        {
            var server = TestServer.Create(app => new OwinStartup().Configuration(app));
            return server.HttpClient;
        }
    }
}
