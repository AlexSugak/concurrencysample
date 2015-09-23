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
        [UseDatabase]
        public void GET_api_home_returns_OK()
        {
            using (var client = TestServerHttpClientFactory.Create())
            {
                var response = client.GetAsync("/api").Result;

                Assert.True(response.IsSuccessStatusCode, "Actual response status code was: " + response.StatusCode);
            }
        }

        [Fact]
        [UseDatabase]
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
        [UseDatabase]
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
        [UseDatabase]
        public void GET_documents_resource_returns_POSTed_document_in_content()
        {
            using (var client = TestServerHttpClientFactory.Create())
            {
                var json = new
                {
                    title = "Sample document",
                    content = "this is a sample document content"
                };

                client.PostAsJsonAsync("/api/documents", json).Wait();

                var response = client.GetAsync("/api/documents").Result;
                var actual = response.Content.ReadAsJsonAsync().Result;

                Assert.Equal(json.title.ToString(), actual.documents[0].title.ToString());
                Assert.Equal(json.content.ToString(), actual.documents[0].content.ToString());
            }
        }

        // now we do the "spike", i.e. tests that force us to implement everything down to the database

        [Fact]
        [UseDatabase]
        public void GET_documents_resouce_returns_document_stored_in_database()
        {
            var document = new
            {
                Id = Guid.NewGuid(),
                Title = "title1",
                Content = "not empty content",
                CheckedOutBy = "bob"
            };

            var expected = document.ToJObject();

            var db = Simple.Data.Database.OpenNamedConnection("DocumentsDBConnectionString");
            db.Documents.Insert(document);

            using (var client = TestServerHttpClientFactory.Create())
            {
                var response = client.GetAsync("/api/documents").Result;
                var actual = response.Content.ReadAsJsonAsync().Result;

                Assert.Contains(expected, actual.documents);
            }
        }

        [Fact]
        [UseDatabase]
        public void POSTed_document_saved_to_database()
        {
            var document = new
            {
                title = "title1",
                content = "not empty content",
            };

            using (var client = TestServerHttpClientFactory.Create())
            {
                client.PostAsJsonAsync("/api/documents", document).Wait();
            }

            var db = Simple.Data.Database.OpenNamedConnection("DocumentsDBConnectionString");
            var dbDocuments = db.Documents.All();

            Assert.Equal(1, dbDocuments.Count());
            Assert.Equal(document.title.ToString(), dbDocuments.First().Title.ToString());
            Assert.Equal(document.content.ToString(), dbDocuments.First().Content.ToString());
        }
    }
}
