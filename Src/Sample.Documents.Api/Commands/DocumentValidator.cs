using System;
using System.Collections.Generic;
using System.Linq;
using Sample.Api.Shared;
using FluentValidation;
using FluentValidation.Results;

namespace Sample.Documents.Api.Commands
{
    /// <summary>
    /// Validates document data before executing provided command
    /// </summary>
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
            if (!result.IsValid)
            {
                throw new Sample.Api.Shared.ValidationException(BuildMessage(result.Errors));
            }

            _implementation.Execute(document);
        }

        private static string BuildMessage(IEnumerable<ValidationFailure> errors)
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
                RuleFor(d => d.Title).Length(1, 100).NotEmpty();
                RuleFor(d => d.Content).NotEmpty();
            }
        }
    }
}
