using System;
using System.Data.SqlClient;
using Sample.Api.Shared;

namespace Sample.Documents.Api.Commands
{
    public class UpdateDocumentSqlCommand : SqlOperation, ICommand<Document>
    {
        public UpdateDocumentSqlCommand(string connectionString)
            : base(connectionString)
        {
        }

        public void Execute(Envelope<Document> document)
        {
            base.ExecuteNonQuery(
                "UPDATE [dbo].[Documents] SET [Title] = @title, [Content] = @content WHERE [Id] = @id",
                new SqlParameter("@id", document.Item.DocumentId),
                new SqlParameter("@title", document.Item.Title),
                new SqlParameter("@content", document.Item.Content));
        }
    }
}
