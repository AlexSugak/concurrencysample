using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Sample.Api.Shared;
using FluentValidation;
using FluentValidation.Results;

namespace Sample.Documents.Api.Commands
{
    public interface IDocumentReference
    {
        Guid DocumentId { get; }
    }

    public class DocumentReference : IDocumentReference
    {
        public DocumentReference(Guid id)
        {
            DocumentId = id;
        }

        public Guid DocumentId { get; private set; }
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
                throw new Sample.Api.Shared.ValidationException(BuildMessage(result.Errors));
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

    public class SubmitNewDocumentSqlCommand : SqlOperation, ICommand<Document>
    {
        public SubmitNewDocumentSqlCommand(string connectionString)
            : base(connectionString)
        {
        }

        public void Execute(Envelope<Document> document)
        {
            base.ExecuteNonQuery(
                "INSERT INTO [dbo].[Documents] ([Id], [Title], [Content]) VALUES (@id, @title, @content)",
                new SqlParameter("@id", document.Item.DocumentId),
                new SqlParameter("@title", document.Item.Title),
                new SqlParameter("@content", document.Item.Content));
        }
    }
}
