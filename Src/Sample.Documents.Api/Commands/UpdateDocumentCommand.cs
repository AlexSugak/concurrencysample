using System;
using System.Data.SqlClient;
using Sample.Api.Shared;

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
                string cmdText = "UPDATE [dbo].[Documents] SET [Title] = @title, [Content] = @content WHERE [Id] = @id";
                using (var cmd = new SqlCommand(cmdText, connection))
                {
                    cmd.Parameters.Add(new SqlParameter("@id", document.Item.DocumentId));
                    cmd.Parameters.Add(new SqlParameter("@title", document.Item.Title));
                    cmd.Parameters.Add(new SqlParameter("@content", document.Item.Content));

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
