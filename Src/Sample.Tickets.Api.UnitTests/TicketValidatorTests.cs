using Moq;
using Sample.Api.Shared;
using Sample.Api.Shared.Tests;
using Sample.Tickets.Api.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Extensions;
using FluentAssertions;

namespace Sample.Tickets.Api.UnitTests
{
    public class TicketValidatorTests
    {
        [Theory]
        [MoqAutoData]
        public void execute_calls_inner_command(
            Envelope<Ticket> ticket,
            Mock<ICommand<Ticket>> inner)
        {
            var sut = new TicketValidator(inner.Object);

            sut.Execute(ticket);

            inner.Verify(i => i.Execute(ticket), Times.Once);
        }

        [Theory]
        [MoqAutoData]
        public void execute_throws_validation_exception_on_missing_id(
            Envelope<Ticket> ticket,
            Mock<ICommand<Ticket>> inner)
        {
            ticket.Item.TicketId = Guid.Empty;
            var sut = new TicketValidator(inner.Object);

            sut.Invoking(s => s.Execute(ticket)).ShouldThrow<ValidationException>();
        }

        [Theory]
        [MoqAutoData]
        public void execute_throws_validation_exception_on_missing_title(
            Envelope<Ticket> ticket,
            Mock<ICommand<Ticket>> inner)
        {
            ticket.Item.Title = null;
            var sut = new TicketValidator(inner.Object);

            sut.Invoking(s => s.Execute(ticket)).ShouldThrow<ValidationException>();
        }

        [Theory]
        [MoqAutoData]
        public void execute_throws_validation_exception_on_title_bigger_than_100_chars(
            Envelope<Ticket> ticket,
            Mock<ICommand<Ticket>> inner)
        {
            ticket.Item.Title = new string('a', 101);
            var sut = new TicketValidator(inner.Object);

            sut.Invoking(s => s.Execute(ticket)).ShouldThrow<ValidationException>();
        }

        [Theory]
        [MoqAutoData]
        public void execute_throws_validation_exception_on_missing_status(
            Envelope<Ticket> ticket,
            Mock<ICommand<Ticket>> inner)
        {
            ticket.Item.Status = null;
            var sut = new TicketValidator(inner.Object);

            sut.Invoking(s => s.Execute(ticket)).ShouldThrow<ValidationException>();
        }

        [Theory]
        [MoqAutoData]
        public void execute_throws_validation_exception_on_status_bigger_than_50_chars(
            Envelope<Ticket> ticket,
            Mock<ICommand<Ticket>> inner)
        {
            ticket.Item.Status = new string('b', 51);
            var sut = new TicketValidator(inner.Object);

            sut.Invoking(s => s.Execute(ticket)).ShouldThrow<ValidationException>();
        }

        [Theory]
        [MoqAutoData]
        public void execute_throws_validation_exception_on_missing_severity(
            Envelope<Ticket> ticket,
            Mock<ICommand<Ticket>> inner)
        {
            ticket.Item.Severity = null;
            var sut = new TicketValidator(inner.Object);

            sut.Invoking(s => s.Execute(ticket)).ShouldThrow<ValidationException>();
        }

        [Theory]
        [MoqAutoData]
        public void execute_throws_validation_exception_on_severity_bigger_than_100_chars(
            Envelope<Ticket> ticket,
            Mock<ICommand<Ticket>> inner)
        {
            ticket.Item.Severity = new string('b', 101);
            var sut = new TicketValidator(inner.Object);

            sut.Invoking(s => s.Execute(ticket)).ShouldThrow<ValidationException>();
        }

        [Theory]
        [MoqAutoData]
        public void execute_throws_validation_exception_on_assigned_to_bigger_than_100_chars(
            Envelope<Ticket> ticket,
            Mock<ICommand<Ticket>> inner)
        {
            ticket.Item.AssignedTo = new string('b', 101);
            var sut = new TicketValidator(inner.Object);

            sut.Invoking(s => s.Execute(ticket)).ShouldThrow<ValidationException>();
        }

        [Theory]
        [MoqAutoData]
        public void execute_throws_validation_exception_on_description_bigger_than_500_chars(
            Envelope<Ticket> ticket,
            Mock<ICommand<Ticket>> inner)
        {
            ticket.Item.Description = new string('b', 501);
            var sut = new TicketValidator(inner.Object);

            sut.Invoking(s => s.Execute(ticket)).ShouldThrow<ValidationException>();
        }
    }
}
