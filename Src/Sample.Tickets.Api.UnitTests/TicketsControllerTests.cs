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
using Sample.Tickets.Api.Commands;
using System.Net;

namespace Sample.Tickets.Api.UnitTests
{
    public class TicketsControllerTests
    {
        [Theory]
        [TicketsControllerAutoData]
        public void get_returns_403_unauthorized_if_no_auth_header(
            [Frozen]Mock<IQuery<EmptyRequest, IEnumerable<TicketDetails>>> ticketsQuery,
            TicketsController sut)
        {
            ticketsQuery.Setup(q => q.Execute(It.IsAny<Envelope<EmptyRequest>>())).Throws<UnauthorizedAccessException>();

            var actual = sut.Get();

            actual.Should().BeOfType<UnauthorizedResult>("because user name query returned nothing");
        }

        [Theory]
        [TicketsControllerAutoData]
        public void get_returns_tickets_returned_by_query(
            List<TicketDetails> tickets,
            [Frozen]Mock<IQuery<EmptyRequest, IEnumerable<TicketDetails>>> ticketsQuery,
            TicketsController sut)
        {
            ticketsQuery.Setup(q => q.Execute(It.IsAny<Envelope<EmptyRequest>>())).Returns(tickets);

            var actual = sut.Get();

            actual.Should().BeOfType<OkNegotiatedContentResult<TicketsModel>>()
                  .Which.Content.Tickets.Select(t => t.Title).Should().Equal(tickets.Select(t => t.Title));
        }

        [Theory]
        [TicketsControllerAutoData]
        public void get_by_id_returns_403_unauthorized_if_no_auth_header(
            Guid ticketId,
            [Frozen]Mock<IQuery<Guid, TicketDetails>> query,
            TicketsController sut)
        {
            query.Setup(q => q.Execute(It.IsAny<Envelope<Guid>>())).Throws<UnauthorizedAccessException>();

            var actual = sut.Get(ticketId);

            actual.Should().BeOfType<UnauthorizedResult>("because user name query returned nothing");
        }

        [Theory]
        [TicketsControllerAutoData]
        public void get_by_id_returns_ticket_returned_by_query(
            string userName,
            Guid ticketId,
            TicketDetails ticket,
            [Frozen]Mock<IQuery<Guid, TicketDetails>> ticketQuery,
            TicketsController sut)
        {
            ticketQuery.Setup(q => q.Execute(It.Is<Envelope<Guid>>(t => t.Item == ticketId))).Returns(ticket);

            var actual = sut.Get(ticketId);

            actual.Should().BeOfType<OkResultWithETag<TicketResponseModel>>()
                  .Which.Content.Should().ShouldBeEquivalentTo(ticket, options => options.ExcludingMissingMembers());
        }

        [Theory]
        [TicketsControllerAutoData]
        public void get_by_id_returns_ticket_version_in_etag_header(
            Guid ticketId,
            TicketDetails ticket,
            [Frozen]Mock<IQuery<Guid, TicketDetails>> ticketQuery,
            TicketsController sut)
        {
            ticketQuery.Setup(q => q.Execute(It.Is<Envelope<Guid>>(t => t.Item == ticketId))).Returns(ticket);

            var actual = sut.Get(ticketId);

            actual.Should().BeOfType<OkResultWithETag<TicketResponseModel>>()
                  .Which.ETagValue.Should().Be(ticket.Version.ToString());
        }

        [Theory]
        [TicketsControllerAutoData]
        public void get_by_id_returns_404_NotFound_when_ticket_not_found(
            Guid ticketId,
            TicketNotFoundException notFound,
            [Frozen]Mock<IQuery<Guid, TicketDetails>> ticketQuery,
            TicketsController sut)
        {
            ticketQuery.Setup(q => q.Execute(It.Is<Envelope<Guid>>(t => t.Item == ticketId))).Throws(notFound);

            var actual = sut.Get(ticketId);

            actual.Should().BeOfType<NotFoundResult>("because TicketNotFoundException was thrown by query");
        }

        [Theory]
        [TicketsControllerAutoData]
        public void post_returns_403_unauthorized_if_not_authorized(
            TicketModel ticket,
            [Frozen]Mock<ICommand<Ticket>> cmd,
            TicketsController sut)
        {
            cmd.Setup(c => c.Execute(It.IsAny<Envelope<Ticket>>())).Throws<UnauthorizedAccessException>();

            var actual = sut.Post(ticket);

            actual.Should().BeOfType<UnauthorizedResult>("because user name query returned nothing");
        }

        [Theory]
        [TicketsControllerAutoData]
        public void post_returns_201_created_with_correct_location_on_success(
            TicketModel ticket,
            [Frozen]Mock<UrlHelper> url,
            TicketsController sut)
        {
            var createdUri = "http://localhost:8051/api/tickets/123";
            url.Setup(u => u.Link(It.IsAny<string>(), It.IsAny<object>()))
                            .Returns(createdUri);

            var actual = sut.Post(ticket);

            actual.Should().BeOfType<CreatedResultWithETag<TicketResponseModel>>()
                  .Which.Location.OriginalString.Should().Be(createdUri);
        }

        [Theory]
        [TicketsControllerAutoData]
        public void post_returns_created_ticket_on_success(
            TicketModel ticket,
            TicketDetails ticketDetails,
            [Frozen]Mock<IQuery<Guid, TicketDetails>> ticketQuery,
            [Frozen]Mock<UrlHelper> url,
            TicketsController sut)
        {
            var createdUri = "http://localhost:8051/api/tickets/123";
            url.Setup(u => u.Link(It.IsAny<string>(), It.IsAny<object>()))
                            .Returns(createdUri);
            ticketQuery.Setup(q => q.Execute(It.IsAny<Envelope<Guid>>())).Returns(ticketDetails);

            var actual = sut.Post(ticket);

            actual.Should().BeOfType<CreatedResultWithETag<TicketResponseModel>>()
                  .Which.Content.ShouldBeEquivalentTo(ticketDetails, options => options.ExcludingMissingMembers());
        }

        [Theory]
        [TicketsControllerAutoData]
        public void post_returns_ticket_etag_on_success(
            TicketModel ticket,
            TicketDetails ticketDetails,
            [Frozen]Mock<IQuery<Guid, TicketDetails>> ticketQuery,
            [Frozen]Mock<UrlHelper> url,
            TicketsController sut)
        {
            var createdUri = "http://localhost:8051/api/tickets/123";
            url.Setup(u => u.Link(It.IsAny<string>(), It.IsAny<object>()))
                            .Returns(createdUri);
            ticketQuery.Setup(q => q.Execute(It.IsAny<Envelope<Guid>>())).Returns(ticketDetails);

            var actual = sut.Post(ticket);

            actual.Should().BeOfType<CreatedResultWithETag<TicketResponseModel>>()
                  .Which.ETagValue.Should().Be(ticketDetails.Version.ToString());
        }

        [Theory]
        [TicketsControllerAutoData]
        public void post_returns_400_bad_request_on_validation_error(
            TicketModel ticket,
            ValidationException exception,
            [Frozen]Mock<ICommand<Ticket>> addCmd,
            TicketsController sut)
        {
            addCmd.Setup(cmd => cmd.Execute(It.IsAny<Envelope<Ticket>>())).Throws(exception);

            var actual = sut.Post(ticket);

            actual.Should().BeOfType<BadRequestErrorMessageResult>();
        }

        [Theory]
        [TicketsControllerAutoData]
        public void put_returns_403_unauthorized_if_unauthorized(
            Guid ticketId,
            TicketModel ticket,
            [Frozen]Mock<ICommand<Ticket>> updateCmd,
            TicketsController sut)
        {
            updateCmd.Setup(c => c.Execute(It.IsAny<Envelope<Ticket>>()))
                     .Throws<UnauthorizedAccessException>();

            var actual = sut.Put(ticketId, ticket);

            actual.Should().BeOfType<UnauthorizedResult>("because user name query returned nothing");
        }

        [Theory]
        [TicketsControllerAutoData]
        public void put_returns_200_ok_with_correct_etag_on_success(
            Guid ticketId,
            TicketModel ticket,
            TicketDetails ticketDetails,
            [Frozen]Mock<IUserNameQuery> userQuery,
            [Frozen]Mock<IQuery<Guid, TicketDetails>> ticketQuery,
            TicketsController sut)
        {
            ticketQuery.Setup(q => q.Execute(It.IsAny<Envelope<Guid>>())).Returns(ticketDetails);

            var actual = sut.Put(ticketId, ticket);

            actual.Should().BeOfType<OkResultWithETag<TicketResponseModel>>()
                .Which.ETagValue.Should().Be(ticketDetails.Version.ToString());
        }

        [Theory]
        [TicketsControllerAutoData]
        public void put_returns_412_precondition_failed_on_version_conflict(
            Guid ticketId,
            TicketModel ticket,
            OptimisticConcurrencyException exception,
            [Frozen]Mock<ICommand<Ticket>> updateCmd,
            TicketsController sut)
        {
            updateCmd.Setup(cmd => cmd.Execute(It.IsAny<Envelope<Ticket>>())).Throws(exception);

            var actual = sut.Put(ticketId, ticket);

            actual.Should().BeOfType<ResponseMessageResult>()
                  .Which.Response.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
        }

        [Theory]
        [TicketsControllerAutoData]
        public void put_returns_404_notfound_when_ticket_not_found(
            Guid ticketId,
            TicketModel ticket,
            TicketNotFoundException exception,
            [Frozen]Mock<ICommand<Ticket>> updateCmd,
            TicketsController sut)
        {
            updateCmd.Setup(cmd => cmd.Execute(It.IsAny<Envelope<Ticket>>())).Throws(exception);

            var actual = sut.Put(ticketId, ticket);

            actual.Should().BeOfType<NotFoundResult>("because TicketNotFoundException was thrown");
        }

        [Theory]
        [TicketsControllerAutoData]
        public void put_returns_400_bad_request_on_validation_error(
            TicketModel ticket,
            Guid ticketId,
            ValidationException exception,
            [Frozen]Mock<ICommand<Ticket>> updateCmd,
            TicketsController sut)
        {
            updateCmd.Setup(cmd => cmd.Execute(It.IsAny<Envelope<Ticket>>())).Throws(exception);

            var actual = sut.Put(ticketId, ticket);

            actual.Should().BeOfType<BadRequestErrorMessageResult>();
        }

        [Theory]
        [TicketsControllerAutoData]
        public void delete_returns_403_unauthorized_if_no_auth_header(
            Guid ticketId,
            [Frozen]Mock<ICommand<TicketReference>> deleteCmd,
            TicketsController sut)
        {
            deleteCmd.Setup(cmd => cmd.Execute(It.IsAny<Envelope<TicketReference>>())).Throws<UnauthorizedAccessException>();

            var actual = sut.Delete(ticketId);

            actual.Should().BeOfType<UnauthorizedResult>("because user name query returned nothing");
        }

        [Theory]
        [TicketsControllerAutoData]
        public void delete_returns_204_no_content_on_success(
            Guid ticketId,
            TicketsController sut)
        {
            var actual = sut.Delete(ticketId);

            actual.Should().BeOfType<ResponseMessageResult>()
                .Which.Response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Theory]
        [TicketsControllerAutoData]
        public void delete_returns_412_precondition_failed_on_version_conflict(
            Guid ticketId,
            OptimisticConcurrencyException exception,
            [Frozen]Mock<ICommand<TicketReference>> deleteCmd,
            TicketsController sut)
        {
            deleteCmd.Setup(cmd => cmd.Execute(It.IsAny<Envelope<TicketReference>>())).Throws(exception);

            var actual = sut.Delete(ticketId);

            actual.Should().BeOfType<ResponseMessageResult>()
                .Which.Response.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
        }
    }
}
