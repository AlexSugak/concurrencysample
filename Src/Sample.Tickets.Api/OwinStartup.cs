using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Owin;
using Sample.Api.Shared;
using Microsoft.Owin.Cors;
using Sample.Tickets.Api.Queries;
using System.Configuration;
using Sample.Tickets.Api.Commands;

namespace Sample.Tickets.Api
{
    /// <summary>
    /// Handles startup of owin-hosted api
    /// </summary>
    public class OwinStartup
    {
        public void Configuration(IAppBuilder app)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

            var userNameQuery = new SimppleTokenUserNameQuery();
            var envelop = new EnvelopWithUserName(userNameQuery);
            var allTicketsQuery = new SecuredQuery<EmptyRequest, IEnumerable<TicketDetails>>(
                                    new GetAllTicketsSqlQuery(connectionString));
            var getTicketQuery = new SecuredQuery<Guid, TicketDetails>(
                                    new GetTicketByIdQuerySqlQuery(connectionString));
            var getVersionQuery = new IfMatchHttpHeaderTicketVersionQuery();
            var addTicketCmd = new TransactedCommand<Ticket>(
                                    new SecuredCommand<Ticket>(
                                        new TicketValidator(
                                            new SubmitNewTicketSqlCommand(connectionString))));
            var updateTicketCmd = new TransactedCommand<Ticket>(
                                        new SecuredCommand<Ticket>(
                                            new TicketValidator(
                                                new UpdateTicketSqlCommand(connectionString))));
            var deleteTicketCmd = new TransactedCommand<TicketReference>(
                                        new SecuredCommand<TicketReference>(
                                            new DeleteTicketSqlCommand(connectionString)));

            var compositon = new CompositionRoot(
                envelop,
                userNameQuery,
                allTicketsQuery,
                getTicketQuery,
                getVersionQuery,
                addTicketCmd,
                updateTicketCmd,
                deleteTicketCmd);

            var config = new HttpConfiguration();
            HttpConfigurator.Configure(config, compositon);

            app.UseCors(CorsOptions.AllowAll);
            app.UseWebApi(config);
        }
    }
}
