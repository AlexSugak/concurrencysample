using Sample.Api.Shared;
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

    public class GetAllDocumentsSqlQuery : SqlOperation, IGetAllDocumentsQuery
    {
        public GetAllDocumentsSqlQuery(string connectionString)
            : base(connectionString)
        {
        }

        public IEnumerable<DocumentDetails> Execute()
        {
            return base.ExecuteReader<DocumentDetails>(
                "SELECT * FROM dbo.Documents ORDER BY [Title]",
                reader => new DocumentDetails()
                            {
                                Id = (Guid)reader["Id"],
                                Title = (string)reader["Title"],
                                Content = (string)reader["Content"],
                                CheckedOutBy = reader["CheckedOutBy"] as string
                            });
        }
    }
}
