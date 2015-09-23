using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Sample.Documents.Api
{
    /// <summary>
    /// Controller for documents resource
    /// </summary>
    [RoutePrefix("api/documents")]
    public class DocumentsController : ApiController
    {
        private string _connectionString;

        public DocumentsController()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DocumentsDBConnectionString"].ConnectionString;
        }

        [Route("")]
        public HttpResponseMessage Get()
        {
            return this.Request.CreateResponse<DocumentsModel>(new DocumentsModel()
            {
                Documents = ReadDocuments().ToArray()
            });
        }

        [Route("")]
        public HttpResponseMessage Post(DocumentModel model)
        {
            WriteDocument(model);
            return this.Request.CreateResponse(HttpStatusCode.Created);
        }

        private void WriteDocument(DocumentModel model)
        {
            using(var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using(var transaction = connection.BeginTransaction())
                {
                    string cmdText = "INSERT INTO dbo.Documents ([Id], [Title], [Content]) VALUES (@id, @title, @content)";
                    using(var cmd = new SqlCommand(cmdText, transaction.Connection, transaction))
                    {
                        cmd.Parameters.Add(new SqlParameter("@id", Guid.NewGuid()));
                        cmd.Parameters.Add(new SqlParameter("@title", model.Title));
                        cmd.Parameters.Add(new SqlParameter("@content", model.Content));

                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
        }

        private IEnumerable<DocumentResponseModel> ReadDocuments()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string cmdText = "SELECT * FROM dbo.Documents";
                using(var cmd = new SqlCommand(cmdText, connection))
                {
                    using(var reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            yield return new DocumentResponseModel() 
                            {
                                Id = reader["Id"].ToString(),
                                Title = (string)reader["Title"],
                                Content = (string)reader["Content"],
                                CheckedOutBy = reader["CheckedOutBy"] as string
                            };
                        }
                    }
                }
            }
        }
    }

    public class DocumentsModel
    {
        public DocumentResponseModel[] Documents { get; set; }
    }

    public class DocumentModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
    }

    public class DocumentResponseModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string CheckedOutBy { get; set; }
    }
}
