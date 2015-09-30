using Sample.Api.Shared;
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

    public class GetDocumentSqlQuery : SqlOperation, IGetDocumentQuery
    {
        public GetDocumentSqlQuery(string connectionString)
            : base(connectionString)
        {
        }

        public DocumentDetails Execute(Guid documentId)
        {
            return base.ExecuteReaderOnce<DocumentDetails>(
                "SELECT * FROM [dbo].[Documents] WHERE [Id] = @id",
                reader => new DocumentDetails()
                            {
                                Id = (Guid)reader["Id"],
                                Title = (string)reader["Title"],
                                Content = (string)reader["Content"],
                                CheckedOutBy = reader["CheckedOutBy"] as string
                            },
                () => new DocumentNotFoundException(string.Format("Document {0} was not found", documentId)),
                new SqlParameter("@id", documentId));
        }
    }
}
