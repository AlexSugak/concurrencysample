using Ploeh.AutoFixture.Xunit;
using Sample.Api.Shared.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Extensions;
using System.Net.Http.Headers;
using System.Net;

namespace Sample.Tickets.Api.IntegrationTests
{
    public class TicketsApiTests
    {
        //breaking the ice

        [Theory]
        [AutoData]
        [UseDatabase]
        public void GET_home_url_returns_OK_result(
            string userName
            )
        {
            using(var client = TestServerHttpClientFactory.Create(userName))
            {
                var response = client.GetAsync("/api").Result;

                Assert.True(response.IsSuccessStatusCode, "Actual response status code was: " + response.StatusCode);
            }
        }

        [Theory]
        [AutoData]
        [UseDatabase]
        public void GET_tickets_returns_OK_result_with_json_content(
            string userName
            )
        {
            using (var client = TestServerHttpClientFactory.Create(userName))
            {
                var response = client.GetAsync("/api/tickets").Result;

                Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);

                var json = response.Content.ReadAsJsonAsync().Result;
                Assert.True(response.IsSuccessStatusCode);
                Assert.NotNull(json);
            }
        }

        [Theory]
        [AutoData]
        [UseDatabase]
        public void GET_tickets_returns_POSTed_ticket_in_content(
            string userName,
            string title,
            string description,
            string severity,
            string status,
            string assignedTo
            )
        {
            using (var client = TestServerHttpClientFactory.Create(userName))
            {
                var json = new
                {
                    title = title,
                    description = description,
                    severity = severity,
                    status = status,
                    assignedTo = assignedTo,
                };

                client.PostAsJsonAsync("/api/tickets", json).Wait();

                var response = client.GetAsync("/api/tickets").Result;

                var actual = response.Content.ReadAsJsonAsync().Result.tickets[0];
                Assert.NotNull(actual);
                Assert.Equal(title, actual.title.ToString());
                Assert.Equal(description, actual.description.ToString());
                Assert.Equal(severity, actual.severity.ToString());
                Assert.Equal(status, actual.status.ToString());
                Assert.Equal(assignedTo, actual.assignedTo.ToString());
            }
        }

        //spike

        private const string ConnectionStringName = "DBConnectionString";

        [Theory]
        [AutoData]
        [UseDatabase]
        public void GET_tickets_resource_returns_ticket_stored_in_the_db(
            Guid ticketId,
            string userName,
            string title,
            string description,
            string severity,
            string assignedTo,
            string status,
            Guid version
            )
        {
            using (var client = TestServerHttpClientFactory.Create(userName))
            {
                var ticket = new
                {
                    Id = ticketId,
                    Title = title,
                    Description = description,
                    AssignedTo = assignedTo,
                    Severity = severity,
                    Status = status,
                    Version = version
                };

                var db = Simple.Data.Database.OpenNamedConnection(ConnectionStringName);
                db.Tickets.Insert(ticket);

                var response = client.GetAsync("/api/tickets").Result;

                var actual = response.Content.ReadAsJsonAsync().Result.tickets[0];
                Assert.NotNull(actual);
                Assert.Equal(ticketId.ToString(), actual.id.ToString());
                Assert.Equal(title, actual.title.ToString());
                Assert.Equal(description, actual.description.ToString());
                Assert.Equal(assignedTo, actual.assignedTo.ToString());
                Assert.Equal(severity, actual.severity.ToString());
                Assert.Equal(status, actual.status.ToString());
            }
        }

        [Theory]
        [AutoData]
        [UseDatabase]
        public void GET_ticket_by_id_returns_ticket_stored_in_the_db(
            Guid ticketId,
            string userName,
            string title,
            string description,
            string severity,
            string assignedTo,
            string status
            )
        {
            using (var client = TestServerHttpClientFactory.Create(userName))
            {
                var ticket = new
                {
                    Id = ticketId,
                    Title = title,
                    Description = description,
                    AssignedTo = assignedTo,
                    Severity = severity,
                    Status = status
                };

                var db = Simple.Data.Database.OpenNamedConnection(ConnectionStringName);
                db.Tickets.Insert(ticket);

                var response = client.GetAsync("/api/tickets/" + ticketId).Result;

                var actual = response.Content.ReadAsJsonAsync().Result;
                Assert.NotNull(actual);
                Assert.Equal(ticketId.ToString(), actual.id.ToString());
                Assert.Equal(title, actual.title.ToString());
                Assert.Equal(description, actual.description.ToString());
                Assert.Equal(assignedTo, actual.assignedTo.ToString());
                Assert.Equal(severity, actual.severity.ToString());
                Assert.Equal(status, actual.status.ToString());
            }
        }

        [Theory]
        [AutoData]
        [UseDatabase]
        public void PUT_ticket_updates_ticket_stored_in_the_db(
            Guid ticketId,
            string userName,
            string title,
            string description,
            string severity,
            string assignedTo,
            string status,
            string newTitle,
            string newDescription,
            string newSeverity,
            string newAssignedTo,
            string newStatus
            )
        {
            using (var client = TestServerHttpClientFactory.Create(userName))
            {
                var ticket = new
                {
                    Id = ticketId,
                    Title = title,
                    Description = description,
                    AssignedTo = assignedTo,
                    Severity = severity,
                    Status = status
                };

                var db = Simple.Data.Database.OpenNamedConnection(ConnectionStringName);
                db.Tickets.Insert(ticket);

                var json = new
                {
                    title = newTitle,
                    description = newDescription,
                    severity = newSeverity,
                    status = newStatus,
                    assignedTo = newAssignedTo,
                };

                var version = client.GetAsync("/api/tickets/" + ticketId).Result.Headers.ETag.Tag;
                client.DefaultRequestHeaders.IfMatch.Add(new EntityTagHeaderValue(version));

                client.PutAsJsonAsync("/api/tickets/" + ticketId, json).Wait();

                var response = client.GetAsync("/api/tickets/" + ticketId).Result;

                var actual = response.Content.ReadAsJsonAsync().Result;
                Assert.NotNull(actual);
                Assert.Equal(newTitle, actual.title.ToString());
                Assert.Equal(newDescription, actual.description.ToString());
                Assert.Equal(newAssignedTo, actual.assignedTo.ToString());
                Assert.Equal(newSeverity, actual.severity.ToString());
                Assert.Equal(newStatus, actual.status.ToString());
            }
        }

        [Theory]
        [AutoData]
        [UseDatabase]
        public void PUT_ticket_increments_version(
            Guid ticketId,
            string userName,
            string title,
            string description,
            string severity,
            string assignedTo,
            string status,
            string newTitle,
            string newDescription,
            string newSeverity,
            string newAssignedTo,
            string newStatus
            )
        {
            using (var client = TestServerHttpClientFactory.Create(userName))
            {
                var ticket = new
                {
                    Id = ticketId,
                    Title = title,
                    Description = description,
                    AssignedTo = assignedTo,
                    Severity = severity,
                    Status = status
                };

                var db = Simple.Data.Database.OpenNamedConnection(ConnectionStringName);
                db.Tickets.Insert(ticket);

                var json = new
                {
                    title = newTitle,
                    description = newDescription,
                    severity = newSeverity,
                    status = newStatus,
                    assignedTo = newAssignedTo,
                };

                var version = client.GetAsync("/api/tickets/" + ticketId).Result.Headers.ETag.Tag;
                client.DefaultRequestHeaders.IfMatch.Add(new EntityTagHeaderValue(version));

                client.PutAsJsonAsync("/api/tickets/" + ticketId, json).Wait();

                var response = client.GetAsync("/api/tickets/" + ticketId).Result;

                var newVersion = response.Headers.ETag.Tag;
                Assert.NotEqual(version, newVersion);
            }
        }

        [Theory]
        [AutoData]
        [UseDatabase]
        public void PUT_ticket_returns_precondition_failed_if_version_doesnt_match(
            Guid ticketId,
            string userName,
            string title,
            string description,
            string severity,
            string assignedTo,
            string status,
            string newTitle,
            string newDescription,
            string newSeverity,
            string newAssignedTo,
            string newStatus
            )
        {
            using (var client = TestServerHttpClientFactory.Create(userName))
            {
                var ticket = new
                {
                    Id = ticketId,
                    Title = title,
                    Description = description,
                    AssignedTo = assignedTo,
                    Severity = severity,
                    Status = status
                };

                var db = Simple.Data.Database.OpenNamedConnection(ConnectionStringName);
                db.Tickets.Insert(ticket);

                var json = new
                {
                    title = newTitle,
                    description = newDescription,
                    severity = newSeverity,
                    status = newStatus,
                    assignedTo = newAssignedTo,
                };

                client.DefaultRequestHeaders.IfMatch.Add(new EntityTagHeaderValue("\"34534543\""));

                var response = client.PutAsJsonAsync("/api/tickets/" + ticketId, json).Result;

                Assert.Equal(HttpStatusCode.PreconditionFailed, response.StatusCode);
            }
        }

        [Theory]
        [AutoData]
        [UseDatabase]
        public void DELETE_ticket_by_id_deletes_ticket_stored_in_the_db(
            Guid ticketId,
            string userName,
            string title,
            string description,
            string severity,
            string assignedTo,
            string status
            )
        {
            using (var client = TestServerHttpClientFactory.Create(userName))
            {
                var ticket = new
                {
                    Id = ticketId,
                    Title = title,
                    Description = description,
                    AssignedTo = assignedTo,
                    Severity = severity,
                    Status = status
                };

                var db = Simple.Data.Database.OpenNamedConnection(ConnectionStringName);
                db.Tickets.Insert(ticket);

                var version = client.GetAsync("/api/tickets/" + ticketId).Result.Headers.ETag.Tag;
                client.DefaultRequestHeaders.IfMatch.Add(new EntityTagHeaderValue(version));

                client.DeleteAsync("/api/tickets/" + ticketId).Wait();

                var response = client.GetAsync("/api/tickets").Result;

                var actual = response.Content.ReadAsJsonAsync().Result;
                Assert.NotNull(actual);
                Assert.Equal(0, actual.tickets.Count);
            }
        }

        [Theory]
        [AutoData]
        [UseDatabase]
        public void DELETE_ticket_by_id_returns_precondition_failed_if_version_doesnt_match(
            Guid ticketId,
            string userName,
            string title,
            string description,
            string severity,
            string assignedTo,
            string status
            )
        {
            using (var client = TestServerHttpClientFactory.Create(userName))
            {
                var ticket = new
                {
                    Id = ticketId,
                    Title = title,
                    Description = description,
                    AssignedTo = assignedTo,
                    Severity = severity,
                    Status = status
                };

                var db = Simple.Data.Database.OpenNamedConnection(ConnectionStringName);
                db.Tickets.Insert(ticket);

                client.DefaultRequestHeaders.IfMatch.Add(new EntityTagHeaderValue("\"456456546\""));

                var response = client.DeleteAsync("/api/tickets/" + ticketId).Result;

                Assert.Equal(HttpStatusCode.PreconditionFailed, response.StatusCode);
            }
        }
    }
}
