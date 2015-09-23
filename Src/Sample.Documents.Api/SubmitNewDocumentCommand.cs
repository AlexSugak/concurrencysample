using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Documents.Api
{
    public interface ISubmitNewDocumentCommand
    {
        void Execute(NewDocument input);
    }

    public class NewDocument
    {
        public NewDocument(Guid id, string title, string content)
        {
            Id = id;
            Title = title;
            Content = content;
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }

    public class SqlSubmitNewDocumentCommand : ISubmitNewDocumentCommand
    {
        private readonly string _connectionString;
        public SqlSubmitNewDocumentCommand(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Execute(NewDocument input)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    string cmdText = "INSERT INTO dbo.Documents ([Id], [Title], [Content]) VALUES (@id, @title, @content)";
                    using (var cmd = new SqlCommand(cmdText, transaction.Connection, transaction))
                    {
                        cmd.Parameters.Add(new SqlParameter("@id", input.Id));
                        cmd.Parameters.Add(new SqlParameter("@title", input.Title));
                        cmd.Parameters.Add(new SqlParameter("@content", input.Content));

                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
        }
    }
}
