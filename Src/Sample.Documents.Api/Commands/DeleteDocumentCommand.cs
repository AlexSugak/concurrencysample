using System;
using System.Data.SqlClient;
using Sample.Api.Shared;

namespace Sample.Documents.Api.Commands
{
    public class DeleteDocumentSqlCommand : SqlOperation, ICommand<DocumentReference>
    {
        public DeleteDocumentSqlCommand(string connectionString)
            : base(connectionString)
        {
        }

        public void Execute(Envelope<DocumentReference> input)
        {
            base.ExecuteNonQuery(
                "DELETE FROM [dbo].[Documents] WHERE [Id] = @id",
                new SqlParameter("@id", input.Item.DocumentId));
        }
    }
}
