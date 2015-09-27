using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Documents.Api.Commands
{
    public class Lock : IDocumentReference
    {
        public Lock(Guid documentId)
        {
            DocumentId = documentId;
        }

        public Guid DocumentId { get; set; }
    }

    public class RemoveLockFromDocumentSqlCommand : ICommand<Lock>
    {
        private readonly string _connectionString;

        public RemoveLockFromDocumentSqlCommand(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Execute(Envelope<Lock> lockInfo)
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
