using Ploeh.AutoFixture.Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;

namespace Sample.Documents.Api.IntegrationTests
{
    public class LockApiTests
    {
        //we've already broke the ice, so let's do the spiking for this api from the start

        [Theory]
        [AutoData]
        [UseDatabase]
        public void PUT_lock_on_document_fills_checked_out_by_db_column(string userName)
        {
            var document = new
            {
                Id = Guid.NewGuid(),
                Title = "title1",
                Content = "not empty content1",
            };
            var db = Simple.Data.Database.OpenNamedConnection("DocumentsDBConnectionString");
            db.Documents.Insert(document);

            using (var client = TestServerHttpClientFactory.Create(userName))
            {
                var lockJson = new { };
                client.PutAsJsonAsync("/api/documents/" + document.Id + "/lock", lockJson).Wait();
            }

            var dbDocument = db.Documents.All().First();

            Assert.Equal(userName, dbDocument.CheckedOutBy);
        }

        [Theory]
        [AutoData]
        [UseDatabase]
        public void PUT_lock_on_document_locked_by_another_user_returns_conflict_and_does_not_change_the_db(
            string userName,
            string anotherUserName)
        {
            var document = new
            {
                Id = Guid.NewGuid(),
                Title = "title1",
                Content = "not empty content1",
                CheckedOutBy = anotherUserName
            };
            var db = Simple.Data.Database.OpenNamedConnection("DocumentsDBConnectionString");
            db.Documents.Insert(document);

            using (var client = TestServerHttpClientFactory.Create(userName))
            {
                var lockJson = new { };
                var response = client.PutAsJsonAsync("/api/documents/" + document.Id + "/lock", lockJson).Result;

                var dbDocument = db.Documents.All().First();
                Assert.Equal(HttpStatusCode.PreconditionFailed, response.StatusCode);
                Assert.Equal(anotherUserName, dbDocument.CheckedOutBy);
            }
        }

        [Theory]
        [AutoData]
        [UseDatabase]
        public void DELETE_lock_on_document_clears_checked_out_by_db_column(
            Guid documentId,
            string userName)
        {
            var document = new
            {
                Id = documentId,
                Title = "title1",
                Content = "not empty content1",
                CheckedOutBy = userName
            };
            var db = Simple.Data.Database.OpenNamedConnection("DocumentsDBConnectionString");
            db.Documents.Insert(document);

            using (var client = TestServerHttpClientFactory.Create(userName))
            {
                var lockJson = new { };
                client.DeleteAsync("/api/documents/" + documentId + "/lock").Wait();
            }

            var dbDocument = db.Documents.All().First();

            Assert.Null(dbDocument.CheckedOutBy);
        }

        [Theory]
        [AutoData]
        [UseDatabase]
        public void DELETE_lock_put_by_another_user_returns_conflict_and_does_not_change_the_db(
            Guid documentId,
            string userName,
            string anotherUser)
        {
            var document = new
            {
                Id = documentId,
                Title = "title1",
                Content = "not empty content1",
                CheckedOutBy = anotherUser
            };
            var db = Simple.Data.Database.OpenNamedConnection("DocumentsDBConnectionString");
            db.Documents.Insert(document);

            using (var client = TestServerHttpClientFactory.Create(userName))
            {
                var lockJson = new { };
                var response = client.DeleteAsync("/api/documents/" + documentId + "/lock").Result;

                var dbDocument = db.Documents.All().First();

                Assert.Equal(HttpStatusCode.PreconditionFailed, response.StatusCode);
                Assert.Equal(anotherUser, dbDocument.CheckedOutBy);
            }
        }
    }
}
