using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Documents.Api.Commands
{
    public class UpdateDocumentSqlCommand : ICommand<Document>
    {
        private readonly string _connectionString;
        public UpdateDocumentSqlCommand(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Execute(Envelope<Document> document)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    string cmdText = "UPDATE [dbo].[Documents] SET [Title] = @title, [Content] = @content WHERE [Id] = @id";
                    using (var cmd = new SqlCommand(cmdText, transaction.Connection, transaction))
                    {
                        cmd.Parameters.Add(new SqlParameter("@id", document.Item.DocumentId));
                        cmd.Parameters.Add(new SqlParameter("@title", document.Item.Title));
                        cmd.Parameters.Add(new SqlParameter("@content", document.Item.Content));

                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
        }
    }
}
