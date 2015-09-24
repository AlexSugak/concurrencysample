using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Documents.Api.Commands
{
    public interface IPutLockOnDocumentCommand
    {
        void Execute(string userName, Guid documentId);
    }

    public class PutLockOnDocumentSqlCommand : IPutLockOnDocumentCommand
    {
        private readonly string _connectionString;

        public PutLockOnDocumentSqlCommand(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Execute(string userName, Guid documentId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    string cmdText = "UPDATE [dbo].[Documents] SET [CheckedOutBy] = @user WHERE [Id] = @id";
                    using (var cmd = new SqlCommand(cmdText, transaction.Connection, transaction))
                    {
                        cmd.Parameters.Add(new SqlParameter("@id", documentId));
                        cmd.Parameters.Add(new SqlParameter("@user", userName));

                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
        }
    }
}
