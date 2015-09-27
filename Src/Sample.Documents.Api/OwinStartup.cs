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
            var userNameQuery = new SimppleTokenUserNameQuery();

            var submitDocCmd = new TransactedCommand<Document>(
                                    new DocumentValidator(
                                        new SubmitNewDocumentSqlCommand(connectionString)));
            var updateDocCmd = new TransactedCommand<Document>(
                                    new DocumentValidator(
                                        new DocumentLockValidator<Document>(
                                            new UpdateDocumentSqlCommand(connectionString),
                                            getDocumentQuery)));
            var putLockCmd = new TransactedCommand<Lock>(
                                    new DocumentLockValidator<Lock>(
                                        new PutLockOnDocumentSqlCommand(connectionString),
                                        getDocumentQuery));
            var removeLockCmd = new TransactedCommand<Lock>(
                                    new DocumentLockValidator<Lock>(
                                        new RemoveLockFromDocumentSqlCommand(connectionString),
                                        getDocumentQuery));
            var deleteDocCmd = new TransactedCommand<DocumentReference>(
                                    new DocumentLockValidator<DocumentReference>(
                                        new DeleteDocumentSqlCommand(connectionString),
                                        getDocumentQuery));

            var config = new HttpConfiguration();
            var compositon = new CompositionRoot(
                                    getAllDocumentsQuery,
                                    getDocumentQuery,
                                    submitDocCmd, 
                                    updateDocCmd,
                                    userNameQuery, 
                                    putLockCmd, 
                                    removeLockCmd,
                                    deleteDocCmd);

            HttpConfigurator.Configure(config, compositon);
            app.UseCors(CorsOptions.AllowAll);
            app.UseWebApi(config);
        }
    }
}
