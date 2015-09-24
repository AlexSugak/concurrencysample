using Ploeh.AutoFixture.Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;

namespace Sample.Documents.Api.IntegrationTests
{
    public class LockApiTests
    {
        //we've already broke the ice, so lets do the spiking for this api from the start

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
        public void DELETE_lock_on_document_clears_checked_out_by_db_column(string userName)
        {
            var document = new
            {
                Id = Guid.NewGuid(),
                Title = "title1",
                Content = "not empty content1",
                CheckedOutBy = userName
            };
            var db = Simple.Data.Database.OpenNamedConnection("DocumentsDBConnectionString");
            db.Documents.Insert(document);

            using (var client = TestServerHttpClientFactory.Create(userName))
            {
                var lockJson = new { };
                client.DeleteAsync("/api/documents/" + document.Id + "/lock").Wait();
            }

            var dbDocument = db.Documents.All().First();

            Assert.Null(dbDocument.CheckedOutBy);
        }
    }
}
