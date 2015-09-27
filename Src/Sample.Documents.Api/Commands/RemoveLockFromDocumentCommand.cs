using System;
using System.Data.SqlClient;
using Sample.Api.Shared;

namespace Sample.Documents.Api.Commands
{
    public class LockInfo : IDocumentReference
    {
        public LockInfo(Guid documentId)
        {
            DocumentId = documentId;
        }

        public Guid DocumentId { get; set; }
    }

    public class RemoveLockFromDocumentSqlCommand : ICommand<LockInfo>
    {
        private readonly string _connectionString;

        public RemoveLockFromDocumentSqlCommand(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Execute(Envelope<LockInfo> lockInfo)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = "UPDATE [dbo].[Documents] SET [CheckedOutBy] = null WHERE [Id] = @id";
                using (var cmd = new SqlCommand(cmdText, connection))
                {
                    cmd.Parameters.Add(new SqlParameter("@id", lockInfo.Item.DocumentId));
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
