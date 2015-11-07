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

            var allTicketsQuery = new GetAllTicketsSqlQuery(connectionString)
                                    .Secured();
            var getTicketQuery = new GetTicketByIdQuerySqlQuery(connectionString)
                                    .Secured();
            var getVersionQuery = new IfMatchHttpHeaderTicketVersionQuery();

            var addTicketCmd = new SubmitNewTicketSqlCommand(connectionString)
                                    .WithTicketValidation()
                                    .Secured()
                                    .Transacted();
            var updateTicketCmd = new UpdateTicketSqlCommand(connectionString)
                                    .WithTicketValidation()
                                    .Secured()
                                    .Transacted();
            var deleteTicketCmd = new DeleteTicketSqlCommand(connectionString)
                                    .Secured()
                                    .Transacted();

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
