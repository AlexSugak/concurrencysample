using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Documents.Api.Commands
{
    public interface ICommand<T>
    {
        void Execute(T input);
    }

    public class Document
    {
        public Document(Guid id, string title, string content)
        {
            Id = id;
            Title = title;
            Content = content;
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }

    public class DocumentValidator : ICommand<Document>
    {
        private readonly ICommand<Document> _implementation;
        private readonly AbstractValidator<Document> _validator;

        public DocumentValidator(ICommand<Document> implementation)
        {
            _implementation = implementation;
            _validator = new Validator();
        }

        public void Execute(Document document)
        {
            var result = _validator.Validate(document);
            if(!result.IsValid)
            {
                throw new ValidationException(BuildMessage(result.Errors));
            }
            
            _implementation.Execute(document);
        }

        private static string BuildMessage(IList<ValidationFailure> errors)
        {
            return String.Format(
                "Document is invalid and cannot be saved: {0}", 
                string.Join("; ", errors.Select(e => e.ErrorMessage)));
        }

        class Validator : AbstractValidator<Document>
        {
            public Validator()
            {
                RuleFor(d => d.Id).NotEmpty();
                RuleFor(d => d.Title).NotEmpty();
                RuleFor(d => d.Content).NotEmpty();
            }
        }
    }

    public class SubmitNewDocumentSqlCommand : ICommand<Document>
    {
        private readonly string _connectionString;
        public SubmitNewDocumentSqlCommand(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Execute(Document document)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    string cmdText = "INSERT INTO [dbo].[Documents] ([Id], [Title], [Content]) VALUES (@id, @title, @content)";
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
