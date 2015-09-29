using Moq;
using Sample.Api.Shared;
using Sample.Api.Shared.Tests;
using Sample.Tickets.Api.Commands;
using Sample.Tickets.Api.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Extensions;
using FluentAssertions;
using Sample.Tickets.Api.Exceptions;

namespace Sample.Tickets.Api.UnitTests
{
    public class TicketConcurrentUpdatesDetectorTests
    {
        [Theory]
        [MoqAutoData]
        public void execute_call_inner_command(
            Envelope<Ticket> ticket,
            TicketDetails existingTicket,
            Mock<IGetTicketByIdQuery> getTicketQuery,
            Mock<ICommand<Ticket>> inner)
        {
            existingTicket.Version = ticket.Item.ExpectedVersion;
            getTicketQuery.Setup(q => q.Execute(ticket.Item.TicketId)).Returns(existingTicket);
            var sut = new TicketConcurrentUpdatesDetector<Ticket>(inner.Object, getTicketQuery.Object);

            sut.Execute(ticket);

            inner.Verify(c => c.Execute(ticket), Times.Once);
        }

        [Theory]
        [MoqAutoData]
        public void execute_throws_concurrency_exception_when_versions_dont_match(
            Envelope<Ticket> ticket,
            TicketDetails existingTicket,
            Mock<IGetTicketByIdQuery> getTicketQuery,
            Mock<ICommand<Ticket>> inner)
        {
            getTicketQuery.Setup(q => q.Execute(ticket.Item.TicketId)).Returns(existingTicket);

            var sut = new TicketConcurrentUpdatesDetector<Ticket>(inner.Object, getTicketQuery.Object);

            sut.Invoking(cmd => cmd.Execute(ticket)).ShouldThrow<OptimisticConcurrencyException>();
        }
    }
}
