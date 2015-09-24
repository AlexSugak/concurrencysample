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

    public class SubmitNewDocumentValidator : ISubmitNewDocumentCommand
    {
        private readonly ISubmitNewDocumentCommand _implementation;
        private readonly AbstractValidator<NewDocument> _validator;

        public SubmitNewDocumentValidator(ISubmitNewDocumentCommand implementation)
        {
            _implementation = implementation;
            _validator = new Validator();
        }

        public void Execute(NewDocument input)
        {
            var result = _validator.Validate(input);
            if(!result.IsValid)
            {
                throw new ValidationException(BuildMessage(result.Errors));
            }
            
            _implementation.Execute(input);
        }

        private static string BuildMessage(IList<ValidationFailure> errors)
        {
            return String.Format(
                "Document is invalid and cannot be saved: {0}", 
                string.Join("; ", errors.Select(e => e.ErrorMessage)));
        }

        class Validator : AbstractValidator<NewDocument>
        {
            public Validator()
            {
                RuleFor(d => d.Id).NotEmpty();
                RuleFor(d => d.Title).NotEmpty();
                RuleFor(d => d.Content).NotEmpty();
            }
        }
    }

    public class SubmitNewDocumentSqlCommand : ISubmitNewDocumentCommand
    {
        private readonly string _connectionString;
        public SubmitNewDocumentSqlCommand(string connectionString)
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
