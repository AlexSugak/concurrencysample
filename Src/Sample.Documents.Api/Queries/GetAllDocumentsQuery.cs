using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Documents.Api.Queries
{
    public interface IGetAllDocumentsQuery
    {
        IEnumerable<DocumentDetails> Execute();
    }

    public class DocumentDetails
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string CheckedOutBy { get; set; }
    }

    public class GetAllDocumentsSqlQuery : IGetAllDocumentsQuery
    {
        private readonly string _connectionString;
        public GetAllDocumentsSqlQuery(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<DocumentDetails> Execute()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string cmdText = "SELECT * FROM dbo.Documents";
                using (var cmd = new SqlCommand(cmdText, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return new DocumentDetails()
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
}
