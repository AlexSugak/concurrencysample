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
        // We start with "ice breaker" tests to force us to create something listening for requests on the other end

        [Fact]
        public void GET_api_home_returns_OK()
        {
            using (var client = TestServerHttpClientFactory.Create())
            {
                var response = client.GetAsync("/api").Result;

                Assert.True(response.IsSuccessStatusCode, "Actual response status code was: " + response.StatusCode);
            }
        }

        [Fact]
        public void GET_documents_resource_returns_json_content()
        {
            using (var client = TestServerHttpClientFactory.Create())
            {
                var response = client.GetAsync("/api/documents").Result;

                Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);

                var json = response.Content.ReadAsJsonAsync().Result;
                Assert.NotNull(json);
            }
        }

        [Fact]
        public void POST_documents_resource_returns_success()
        {
            using (var client = TestServerHttpClientFactory.Create())
            {
                var json = new 
                {
                    title = "Sample document",
                    content = "this is a sample document content"
                };

                var response = client.PostAsJsonAsync("/api/documents", json).Result;

                Assert.True(response.IsSuccessStatusCode, "Actual response status code was: " + response.StatusCode);
            }
        }

        [Fact]
        public void GET_documents_resource_returns_POSTed_document_in_content()
        {
            using (var client = TestServerHttpClientFactory.Create())
            {
                var json = new
                {
                    title = "Sample document",
                    content = "this is a sample document content"
                };
                var expected = json.ToJObject();

                client.PostAsJsonAsync("/documents", json).Wait();

                var response = client.GetAsync("/api/documents").Result;
                var actual = response.Content.ReadAsJsonAsync().Result;

                Assert.Contains(expected, actual.documents);
            }
        }
    }
}
