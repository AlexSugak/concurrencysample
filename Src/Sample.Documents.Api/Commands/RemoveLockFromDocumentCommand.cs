using System;
using System.Data.SqlClient;
using Sample.Api.Shared;

namespace Sample.Documents.Api.Commands
{
    public class RemoveLockFromDocumentSqlCommand : SqlOperation, ICommand<LockInfo>
    {
        public RemoveLockFromDocumentSqlCommand(string connectionString)
            : base(connectionString)
        {
        }

        public void Execute(Envelope<LockInfo> lockInfo)
        {
            base.ExecuteNonQuery(
                "UPDATE [dbo].[Documents] SET [CheckedOutBy] = null WHERE [Id] = @id",
                new SqlParameter("@id", lockInfo.Item.DocumentId));
        }
    }
}
