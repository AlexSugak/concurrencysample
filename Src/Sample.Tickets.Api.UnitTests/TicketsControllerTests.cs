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
using System.Web.Http.Routing;

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

        [Theory]
        [TicketsControllerAutoData]
        public void post_returns_403_unauthorized_if_no_auth_header(
            TicketModel ticket,
            TicketsController sut)
        {
            var actual = sut.Post(ticket);

            actual.Should().BeOfType<UnauthorizedResult>("because user name query returned nothing");
        }

        [Theory]
        [TicketsControllerAutoData]
        public void post_returns_201_created_with_correct_location_on_success(
            string userName,
            TicketModel ticket,
            [Frozen]Mock<IUserNameQuery> userQuery,
            [Frozen]Mock<UrlHelper> url,
            TicketsController sut)
        {
            var createdUri = "http://localhost:8051/api/tickets/123";
            url.Setup(u => u.Link(It.IsAny<string>(), It.IsAny<object>()))
                            .Returns(createdUri);
            userQuery.Setup(q => q.Execute(It.IsAny<HttpRequestMessage>())).Returns(userName);

            var actual = sut.Post(ticket);

            actual.Should().BeOfType<CreatedResultWithETag<TicketResponseModel>>()
                  .Which.Location.OriginalString.Should().Be(createdUri);
        }

        [Theory]
        [TicketsControllerAutoData]
        public void post_returns_created_ticket_on_success(
            string userName,
            TicketModel ticket,
            TicketDetails ticketDetails,
            [Frozen]Mock<IUserNameQuery> userQuery,
            [Frozen]Mock<IGetTicketByIdQuery> ticketQuery,
            [Frozen]Mock<UrlHelper> url,
            TicketsController sut)
        {
            var createdUri = "http://localhost:8051/api/tickets/123";
            url.Setup(u => u.Link(It.IsAny<string>(), It.IsAny<object>()))
                            .Returns(createdUri);
            userQuery.Setup(q => q.Execute(It.IsAny<HttpRequestMessage>())).Returns(userName);
            ticketQuery.Setup(q => q.Execute(It.IsAny<Guid>())).Returns(ticketDetails);

            var actual = sut.Post(ticket);

            actual.Should().BeOfType<CreatedResultWithETag<TicketResponseModel>>()
                  .Which.Content.ShouldBeEquivalentTo(ticketDetails, options => options.ExcludingMissingMembers());
        }

        [Theory]
        [TicketsControllerAutoData]
        public void post_returns_ticket_etag_on_success(
            string userName,
            TicketModel ticket,
            TicketDetails ticketDetails,
            [Frozen]Mock<IUserNameQuery> userQuery,
            [Frozen]Mock<IGetTicketByIdQuery> ticketQuery,
            [Frozen]Mock<UrlHelper> url,
            TicketsController sut)
        {
            var createdUri = "http://localhost:8051/api/tickets/123";
            url.Setup(u => u.Link(It.IsAny<string>(), It.IsAny<object>()))
                            .Returns(createdUri);
            userQuery.Setup(q => q.Execute(It.IsAny<HttpRequestMessage>())).Returns(userName);
            ticketQuery.Setup(q => q.Execute(It.IsAny<Guid>())).Returns(ticketDetails);

            var actual = sut.Post(ticket);


            actual.Should().BeOfType<CreatedResultWithETag<TicketResponseModel>>()
                  .Which.ETagValue.Should().Be(ticketDetails.Version.ToString());
        }
    }
}
