using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Documents.Api.Commands
{
    public interface IUpdateDocumentCommand
    {
        void Execute(UpdatedDocument document);
    }

    public class UpdatedDocument
    {
        public UpdatedDocument(Guid id, string title, string content)
        {
            Id = id;
            Title = title;
            Content = content;
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }

    public class UpdateDocumentSqlCommand : IUpdateDocumentCommand
    {
        private readonly string _connectionString;
        public UpdateDocumentSqlCommand(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Execute(UpdatedDocument document)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    string cmdText = "UPDATE [dbo].[Documents] SET [Title] = @title, [Content] = @content WHERE [Id] = @id";
                    using (var cmd = new SqlCommand(cmdText, transaction.Connection, transaction))
                    {
                        cmd.Parameters.Add(new SqlParameter("@id", document.Id));
                        cmd.Parameters.Add(new SqlParameter("@title", document.Title));
                        cmd.Parameters.Add(new SqlParameter("@content", document.Content));

                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
        }
    }
}
