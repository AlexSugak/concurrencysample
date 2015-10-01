using System;
using System.Data.SqlClient;
using Sample.Api.Shared;

namespace Sample.Documents.Api.Commands
{
    public class SubmitNewDocumentSqlCommand : SqlOperation, ICommand<Document>
    {
        public SubmitNewDocumentSqlCommand(string connectionString)
            : base(connectionString)
        {
        }

        public void Execute(Envelope<Document> document)
        {
            base.ExecuteNonQuery(
                "INSERT INTO [dbo].[Documents] ([Id], [Title], [Content]) VALUES (@id, @title, @content)",
                new SqlParameter("@id", document.Item.DocumentId),
                new SqlParameter("@title", document.Item.Title),
                new SqlParameter("@content", document.Item.Content));
        }
    }
}
