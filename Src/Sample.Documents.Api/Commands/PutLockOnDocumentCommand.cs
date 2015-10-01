using System;
using System.Data.SqlClient;
using Sample.Api.Shared;
using Sample.Documents.Api.Exceptions;
using Sample.Documents.Api.Queries;

namespace Sample.Documents.Api.Commands
{
    public class PutLockOnDocumentSqlCommand : SqlOperation, ICommand<LockInfo>
    {
        public PutLockOnDocumentSqlCommand(string connectionString)
            : base(connectionString)
        {
        }

        public void Execute(Envelope<LockInfo> lockInfo)
        {
            base.ExecuteNonQuery(
                "UPDATE [dbo].[Documents] SET [CheckedOutBy] = @user WHERE [Id] = @id",
                new SqlParameter("@id", lockInfo.Item.DocumentId),
                new SqlParameter("@user", lockInfo.UserName));
        }
    }
}
