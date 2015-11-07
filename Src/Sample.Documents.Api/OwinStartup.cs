using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.Http;
using Owin;
using Sample.Api.Shared;
using Sample.Documents.Api.Commands;
using Sample.Documents.Api.Queries;
using Microsoft.Owin.Cors;

namespace Sample.Documents.Api
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

            var getAllDocumentsQuery = new GetAllDocumentsSqlQuery(connectionString)
                                    .Secured();
            var getDocumentQuery = new GetDocumentSqlQuery(connectionString)
                                    .Secured();

            var submitDocCmd = new SubmitNewDocumentSqlCommand(connectionString)
                                    .WithDocumentValidation()
                                    .Secured()
                                    .Transacted();
            var updateDocCmd = new UpdateDocumentSqlCommand(connectionString)
                                    .WithLockValidation(getDocumentQuery)
                                    .WithDocumentValidation()
                                    .Secured()
                                    .Transacted();
            var putLockCmd = new PutLockOnDocumentSqlCommand(connectionString)
                                    .WithLockValidation(getDocumentQuery)
                                    .Secured()
                                    .Transacted();
            var removeLockCmd = new RemoveLockFromDocumentSqlCommand(connectionString)
                                    .WithLockValidation(getDocumentQuery)
                                    .Secured()
                                    .Transacted();
            var deleteDocCmd = new DeleteDocumentSqlCommand(connectionString)
                                    .WithLockValidation(getDocumentQuery)
                                    .Secured()
                                    .Transacted();

            var config = new HttpConfiguration();
            var compositon = new CompositionRoot(
                                    envelop,
                                    getAllDocumentsQuery,
                                    getDocumentQuery,
                                    submitDocCmd, 
                                    updateDocCmd,
                                    putLockCmd, 
                                    removeLockCmd,
                                    deleteDocCmd);

            HttpConfigurator.Configure(config, compositon);
            app.UseCors(CorsOptions.AllowAll);
            app.UseWebApi(config);
        }
    }
}
