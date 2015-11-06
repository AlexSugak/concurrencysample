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
            var envelop = new EnvelopeWithUserName(userNameQuery);

            var getAllDocumentsQuery = new SecuredQuery<EmptyRequest, IEnumerable<DocumentDetails>>(
                                            new GetAllDocumentsSqlQuery(connectionString));
            var getDocumentQuery = new SecuredQuery<Guid, DocumentDetails>(
                                        new GetDocumentSqlQuery(connectionString));

            var submitDocCmd = new TransactedCommand<Document>(
                                    new SecuredCommand<Document>(
                                        new DocumentValidator(
                                            new SubmitNewDocumentSqlCommand(connectionString))));
            var updateDocCmd = new TransactedCommand<Document>(
                                    new SecuredCommand<Document>(
                                        new DocumentValidator(
                                            new DocumentLockValidator<Document>(
                                                new UpdateDocumentSqlCommand(connectionString),
                                                getDocumentQuery))));
            var putLockCmd = new TransactedCommand<LockInfo>(
                                    new SecuredCommand<LockInfo>(
                                        new DocumentLockValidator<LockInfo>(
                                            new PutLockOnDocumentSqlCommand(connectionString),
                                            getDocumentQuery)));
            var removeLockCmd = new TransactedCommand<LockInfo>(
                                    new SecuredCommand<LockInfo>(
                                        new DocumentLockValidator<LockInfo>(
                                            new RemoveLockFromDocumentSqlCommand(connectionString),
                                            getDocumentQuery)));
            var deleteDocCmd = new TransactedCommand<DocumentReference>(
                                    new SecuredCommand<DocumentReference>(
                                        new DocumentLockValidator<DocumentReference>(
                                            new DeleteDocumentSqlCommand(connectionString),
                                            getDocumentQuery)));

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
