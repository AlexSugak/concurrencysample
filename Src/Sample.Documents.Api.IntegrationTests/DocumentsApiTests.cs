using Microsoft.Owin.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sample.Documents.Api.IntegrationTests
{
    /// <summary>
    /// Integration tests for documents api
    /// </summary>
    public class DocumentsApiTests
    {
        /// <summary>
        /// We start with "ice breaker" test to force us to create something listening for requests on the other end
        /// </summary>
        [Fact]
        public void GET_api_home_returns_OK()
        {
            var server = TestServer.Create(app => new OwinStartup().Configuration(app));
            var client = server.HttpClient;
            try
            {
                using(client)
                {
                    var response = client.GetAsync("/api").Result;

                    Assert.True(response.IsSuccessStatusCode, "Actual response status code was: " + response.StatusCode);
                }
            }
            catch
            {
                client.Dispose();
                throw;
            }
        }
    }
}
