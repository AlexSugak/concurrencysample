using System;
using System.Data.SqlClient;
using Sample.Api.Shared;

namespace Sample.Documents.Api.Commands
{
    public class DeleteDocumentSqlCommand : ICommand<DocumentReference>
    {
        private readonly string _connectionString;

        public DeleteDocumentSqlCommand(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Execute(Envelope<DocumentReference> input)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    string cmdText = "DELETE FROM [dbo].[Documents] WHERE [Id] = @id";
                    using (var cmd = new SqlCommand(cmdText, transaction.Connection, transaction))
                    {
                        cmd.Parameters.Add(new SqlParameter("@id", input.Item.DocumentId));
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
        }
    }
}
