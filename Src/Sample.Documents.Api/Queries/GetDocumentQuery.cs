using Sample.Documents.Api.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Documents.Api.Queries
{
    public interface IGetDocumentQuery
    {
        DocumentDetails Execute(Guid documentId);
    }

    public class GetDocumentSqlQuery : IGetDocumentQuery
    {
        private readonly string _connectionString;

        public GetDocumentSqlQuery(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DocumentDetails Execute(Guid documentId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string cmdText = "SELECT * FROM [dbo].[Documents] WHERE [Id] = @id";
                using (var cmd = new SqlCommand(cmdText, connection))
                {
                    cmd.Parameters.Add(new SqlParameter("@id", documentId));

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            throw new DocumentNotFoundException(string.Format("Document {0} was not found", documentId));
                        }

                        reader.Read();
                        return new DocumentDetails()
                        {
                            Id = (Guid)reader["Id"],
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
