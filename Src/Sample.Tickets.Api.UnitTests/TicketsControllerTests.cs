using System;
using System.Collections.Generic;
using System.Linq;
using Sample.Tickets.Api.Controllers;
using FluentAssertions;
using Xunit.Extensions;
using System.Web.Http.Results;
using Ploeh.AutoFixture.Xunit;
using Moq;
using Sample.Tickets.Api.Queries;
using Sample.Api.Shared;
using System.Net.Http;
using Sample.Tickets.Api.Exceptions;

namespace Sample.Tickets.Api.UnitTests
{
    public class TicketsControllerTests
    {
        [Theory]
        [TicketsControllerAutoData]
        public void get_returns_403_unauthorized_if_no_auth_header(
            TicketsController sut)
        {
            var actual = sut.Get();

            actual.Should().BeOfType<UnauthorizedResult>("because user name query returned nothing");
        }

        [Theory]
        [TicketsControllerAutoData]
        public void get_returns_tickets_returned_by_query(
            string userName,
            List<TicketDetails> tickets,
            [Frozen]Mock<IUserNameQuery> userQuery,
            [Frozen]Mock<IGetAllTicketsQuery> ticketsQuery,
            TicketsController sut)
        {
            userQuery.Setup(q => q.Execute(It.IsAny<HttpRequestMessage>())).Returns(userName);
            ticketsQuery.Setup(q => q.Execute()).Returns(tickets);

            var actual = sut.Get();

            actual.Should().BeOfType<OkNegotiatedContentResult<TicketsModel>>()
                .Which.Content.Tickets.Select(t => t.Title).Should().Equal(tickets.Select(t => t.Title));
        }

        [Theory]
        [TicketsControllerAutoData]
        public void get_by_id_returns_403_unauthorized_if_no_auth_header(
            Guid ticketId,
            TicketsController sut)
        {
            var actual = sut.Get(ticketId);

            actual.Should().BeOfType<UnauthorizedResult>("because user name query returned nothing");
        }

        [Theory]
        [TicketsControllerAutoData]
        public void get_by_id_returns_ticket_returned_by_query(
            string userName,
            Guid ticketId,
            TicketDetails ticket,
            [Frozen]Mock<IUserNameQuery> userQuery,
            [Frozen]Mock<IGetTicketByIdQuery> ticketQuery,
            TicketsController sut)
        {
            userQuery.Setup(q => q.Execute(It.IsAny<HttpRequestMessage>())).Returns(userName);
            ticketQuery.Setup(q => q.Execute(ticketId)).Returns(ticket);

            var actual = sut.Get(ticketId);

            actual.Should().BeOfType<OkResultWithETag<TicketResponseModel>>()
                .Which.Content.Should().ShouldBeEquivalentTo(ticket, options => options.ExcludingMissingMembers());
        }

        [Theory]
        [TicketsControllerAutoData]
        public void get_by_id_returns_ticket_version_in_etag_header(
            string userName,
            Guid ticketId,
            TicketDetails ticket,
            [Frozen]Mock<IUserNameQuery> userQuery,
            [Frozen]Mock<IGetTicketByIdQuery> ticketQuery,
            TicketsController sut)
        {
            userQuery.Setup(q => q.Execute(It.IsAny<HttpRequestMessage>())).Returns(userName);
            ticketQuery.Setup(q => q.Execute(ticketId)).Returns(ticket);

            var actual = sut.Get(ticketId);

            actual.Should().BeOfType<OkResultWithETag<TicketResponseModel>>()
                .Which.ETagValue.Should().Be(ticket.Version.ToString());
        }

        [Theory]
        [TicketsControllerAutoData]
        public void get_by_id_returns_404_NotFound_when_ticket_not_found(
            string userName,
            Guid ticketId,
            TicketNotFoundException notFound,
            [Frozen]Mock<IUserNameQuery> userQuery,
            [Frozen]Mock<IGetTicketByIdQuery> ticketQuery,
            TicketsController sut)
        {
            userQuery.Setup(q => q.Execute(It.IsAny<HttpRequestMessage>())).Returns(userName);
            ticketQuery.Setup(q => q.Execute(ticketId)).Throws(notFound);

            var actual = sut.Get(ticketId);

            actual.Should().BeOfType<NotFoundResult>("because TicketNotFoundException was thrown by query");
        }
    }
}
