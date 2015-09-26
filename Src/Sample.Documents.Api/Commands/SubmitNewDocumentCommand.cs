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
    public class Envelope<T>
    {
        public Envelope(T item, string userName)
        {
            Item = item;
            UserName = userName;
        }

        public T Item { get; private set; }
        public string UserName { get; private set; }
    }

    public interface ICommand<T>
    {
        //TODO: implement EnvelopCommand: ICommand<T> -> ICommand<Envelope<T>>
        void Execute(Envelope<T> input);
    }

    public interface IDocumentReference
    {
        Guid DocumentId { get; set; }
    }

    public class Document : IDocumentReference
    {
        public Document(Guid id, string title, string content)
        {
            DocumentId = id;
            Title = title;
            Content = content;
        }

        public Guid DocumentId { get; set; }
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

        public void Execute(Envelope<Document> document)
        {
            var result = _validator.Validate(document.Item);
            if(!result.IsValid)
            {
                throw new Sample.Documents.Api.Exceptions.ValidationException(BuildMessage(result.Errors));
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
                RuleFor(d => d.DocumentId).NotEmpty();
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

        public void Execute(Envelope<Document> document)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    string cmdText = "INSERT INTO [dbo].[Documents] ([Id], [Title], [Content]) VALUES (@id, @title, @content)";
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
