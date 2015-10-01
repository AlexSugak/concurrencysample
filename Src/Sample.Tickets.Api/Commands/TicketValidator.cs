using FluentValidation;
using FluentValidation.Results;
using Sample.Api.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Tickets.Api.Commands
{
    /// <summary>
    /// Validates ticket data before executing provided command
    /// </summary>
    public class TicketValidator : ICommand<Ticket>
    {
        private readonly ICommand<Ticket> _implementation;
        private readonly AbstractValidator<Ticket> _validator;

        public TicketValidator(ICommand<Ticket> implementation)
        {
            _validator = new Validator();
            _implementation = implementation;
        }

        public void Execute(Envelope<Ticket> input)
        {
            var result = _validator.Validate(input.Item);

            if(!result.IsValid)
            {
                throw new Sample.Api.Shared.ValidationException(BuildMessage(result.Errors));
            }

            _implementation.Execute(input);
        }

        private static string BuildMessage(IList<ValidationFailure> errors)
        {
            return String.Format(
                "Ticket is invalid and cannot be saved: {0}",
                string.Join("; ", errors.Select(e => e.ErrorMessage)));
        }

        class Validator : AbstractValidator<Ticket>
        {
            public Validator()
            {
                RuleFor(t => t.TicketId).NotEmpty();
                RuleFor(t => t.Title).NotEmpty().Length(1, 100);
                RuleFor(t => t.Status).NotEmpty().Length(1, 50);
                RuleFor(t => t.Severity).NotEmpty().Length(1, 100);
                RuleFor(t => t.AssignedTo).NotEmpty().Length(0, 100);
                RuleFor(t => t.Description).NotEmpty().Length(0, 100);
            }
        }
    }
}
