using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.Http;
using Owin;
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
            var connectionString = ConfigurationManager.ConnectionStrings["DocumentsDBConnectionString"].ConnectionString;

            var getAllDocumentsQuery = new GetAllDocumentsSqlQuery(connectionString);
            var getDocumentQuery = new GetDocumentSqlQuery(connectionString);
            var submitDocCmd = new DocumentValidator(
                                    new SubmitNewDocumentSqlCommand(connectionString));
            var updateDocCmd = new LockCommandValidator<Document>(
                                    new DocumentValidator(
                                        new UpdateDocumentSqlCommand(connectionString)),
                                    getDocumentQuery);
            var userNameQuery = new SimppleTokenUserNameQuery();
            var putLockCmd = new LockCommandValidator<Lock>(
                                    new PutLockOnDocumentSqlCommand(connectionString),
                                    getDocumentQuery);
            var removeLockCmd = new LockCommandValidator<Lock>(
                                    new RemoveLockFromDocumentSqlCommand(connectionString),
                                    getDocumentQuery);

            var config = new HttpConfiguration();
            var compositon = new CompositionRoot(
                                    getAllDocumentsQuery,
                                    getDocumentQuery,
                                    submitDocCmd, 
                                    updateDocCmd,
                                    userNameQuery, 
                                    putLockCmd, 
                                    removeLockCmd);

            HttpConfigurator.Configure(config, compositon);
            app.UseCors(CorsOptions.AllowAll);
            app.UseWebApi(config);
        }
    }
}
