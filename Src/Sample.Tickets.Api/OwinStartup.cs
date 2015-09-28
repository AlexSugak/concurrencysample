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
            var allTicketsQuery = new GetAllTicketsSqlQuery(connectionString);
            var getTicketQuery = new GetTicketByIdQuerySqlQuery(connectionString);
            var addTicketCmd = new SubmitNewTicketSqlCommand(connectionString);
            var updateTicketCmd = new UpdateTicketSqlCommand(connectionString);
            var deleteTicketCmd = new DeleteTicketSqlCommand(connectionString);

            var compositon = new CompositionRoot(
                userNameQuery, 
                allTicketsQuery, 
                getTicketQuery, 
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
