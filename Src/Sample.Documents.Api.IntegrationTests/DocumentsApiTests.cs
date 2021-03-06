﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Owin.Testing;
using Ploeh.AutoFixture.Xunit;
using Sample.Api.Shared.Tests;
using Xunit;
using Xunit.Extensions;

namespace Sample.Documents.Api.IntegrationTests
{
    /// <summary>
    /// Integration tests for documents api
    /// </summary>
    public class DocumentsApiTests
    {
        // We start with "ice breaker" tests to force us to create something listening for requests on the other end

        [Theory]
        [AutoData]
        [UseDatabase]
        public void GET_api_home_returns_OK(string userName)
        {
            using (var client = TestServerHttpClientFactory.Create(userName))
            {
                var response = client.GetAsync("/api").Result;

                Assert.True(response.IsSuccessStatusCode, "Actual response status code was: " + response.StatusCode);
            }
        }

        [Fact]
        [UseDatabase]
        public void GET_documents_returns_401_when_user_not_specified()
        {
            using (var client = TestServerHttpClientFactory.Create(null))
            {
                var response = client.GetAsync("/api/documents").Result;

                Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            }
        }

        [Theory]
        [AutoData]
        [UseDatabase]
        public void GET_documents_resource_returns_json_content(string userName)
        {
            using (var client = TestServerHttpClientFactory.Create(userName))
            {
                var response = client.GetAsync("/api/documents").Result;

                Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);

                var json = response.Content.ReadAsJsonAsync().Result;
                Assert.NotNull(json);
            }
        }

        [Theory]
        [AutoData]
        [UseDatabase]
        public void POST_documents_resource_returns_success(string userName)
        {
            using (var client = TestServerHttpClientFactory.Create(userName))
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

        [Theory]
        [AutoData]
        [UseDatabase]
        public void GET_documents_resource_returns_POSTed_document_in_content(string userName)
        {
            using (var client = TestServerHttpClientFactory.Create(userName))
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

        private const string ConnectionStringName = "DBConnectionString";

        [Theory]
        [AutoData]
        [UseDatabase]
        public void GET_documents_resouce_returns_document_stored_in_database(
            string userName, 
            string title, 
            string content,
            string checkedOutBy)
        {
            var document = new
            {
                Id = Guid.NewGuid(),
                Title = title,
                Content = content,
                CheckedOutBy = checkedOutBy
            };

            var expected = document.ToJObject();

            var db = Simple.Data.Database.OpenNamedConnection(ConnectionStringName);
            db.Documents.Insert(document);

            using (var client = TestServerHttpClientFactory.Create(userName))
            {
                var response = client.GetAsync("/api/documents").Result;
                var actual = response.Content.ReadAsJsonAsync().Result;

                Assert.Contains(expected, actual.documents);
            }
        }

        [Theory]
        [AutoData]
        [UseDatabase]
        public void POST_returns_401_when_no_user_specified(string title, string content)
        {
            var document = new
            {
                title = title,
                content = content,
            };

            using (var client = TestServerHttpClientFactory.Create(null))
            {
                var response = client.PostAsJsonAsync("/api/documents", document).Result;

                Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            }
        }

        [Theory]
        [AutoData]
        [UseDatabase]
        public void POSTed_document_saved_to_database(string userName, string title, string content)
        {
            var document = new
            {
                title = title,
                content = content,
            };

            using (var client = TestServerHttpClientFactory.Create(userName))
            {
                client.PostAsJsonAsync("/api/documents", document).Wait();
            }

            var db = Simple.Data.Database.OpenNamedConnection(ConnectionStringName);
            var dbDocuments = db.Documents.All();

            Assert.Equal(1, dbDocuments.Count());
            Assert.Equal(title, dbDocuments.First().Title.ToString());
            Assert.Equal(content, dbDocuments.First().Content.ToString());
        }

        [Theory]
        [AutoData]
        [UseDatabase]
        public void PUT_updates_document_in_the_database_when_it_is_checked_out_by_user(
            Guid docId,
            string userName,
            string oldTitle,
            string oldContent,
            string newTitle,
            string newContent)
        {
            var dbDocument = new
            {
                Id = docId,
                Title = oldTitle,
                Content = oldContent,
                CheckedOutBy = userName
            };

            var db = Simple.Data.Database.OpenNamedConnection(ConnectionStringName);
            db.Documents.Insert(dbDocument);

            var updatedDocument = new
            {
                title = newTitle,
                content = newContent,
            };

            using (var client = TestServerHttpClientFactory.Create(userName))
            {
                client.PutAsJsonAsync("/api/documents/" + docId, updatedDocument).Wait();
            }

            var dbDocuments = db.Documents.All();

            Assert.Equal(1, dbDocuments.Count());
            Assert.Equal(newTitle, dbDocuments.First().Title.ToString());
            Assert.Equal(newContent, dbDocuments.First().Content.ToString());
        }

        [Theory]
        [AutoData]
        [UseDatabase]
        public void PUT_returns_conflict_and_does_not_update_document_when_document_checked_out_by_another_user(
            Guid docId,
            string userName,
            string anotherUserName,
            string oldTitle,
            string oldContent,
            string newTitle,
            string newContent)
        {
            var dbDocument = new
            {
                Id = docId,
                Title = oldTitle,
                Content = oldContent,
                CheckedOutBy = anotherUserName
            };

            var db = Simple.Data.Database.OpenNamedConnection(ConnectionStringName);
            db.Documents.Insert(dbDocument);

            var updatedDocument = new
            {
                title = newTitle,
                content = newContent,
            };

            using (var client = TestServerHttpClientFactory.Create(userName))
            {
                var response = client.PutAsJsonAsync("/api/documents/" + docId, updatedDocument).Result;

                Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

                var dbDocuments = db.Documents.All();
                Assert.Equal(1, dbDocuments.Count());
                Assert.Equal(oldTitle, dbDocuments.First().Title.ToString());
                Assert.Equal(oldContent, dbDocuments.First().Content.ToString());
            }
        }

        [Theory]
        [AutoData]
        [UseDatabase]
        public void GET_by_id_returns_document_in_db(
            Guid docId,
            string userName,
            string title,
            string content)
        {
            var dbDocument = new
            {
                Id = docId,
                Title = title,
                Content = content,
                CheckedOutBy = userName
            };

            var db = Simple.Data.Database.OpenNamedConnection(ConnectionStringName);
            db.Documents.Insert(dbDocument);

            using (var client = TestServerHttpClientFactory.Create(userName))
            {
                var response = client.GetAsync("/api/documents/" + docId).Result;

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var actual = response.Content.ReadAsJsonAsync().Result;

                Assert.Equal(title, actual.title.ToString());
                Assert.Equal(content, actual.content.ToString());
                Assert.Equal(userName, actual.checkedOutBy.ToString());
            }
        }

        [Theory]
        [AutoData]
        [UseDatabase]
        public void DELETE_by_id_deletes_document_in_db(
            Guid docId,
            string userName,
            string title,
            string content)
        {
            var dbDocument = new
            {
                Id = docId,
                Title = title,
                Content = content,
                CheckedOutBy = userName
            };

            var db = Simple.Data.Database.OpenNamedConnection(ConnectionStringName);
            db.Documents.Insert(dbDocument);

            using (var client = TestServerHttpClientFactory.Create(userName))
            {
                client.DeleteAsync("/api/documents/" + docId).Wait();

                var dbDocuments = db.Documents.All();
                Assert.Equal(0, dbDocuments.Count());
            }
        }
    }
}
