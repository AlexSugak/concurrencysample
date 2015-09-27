using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Sample.Api.Shared;
using Sample.Documents.Api.Commands;
using Sample.Documents.Api.Exceptions;
using Sample.Documents.Api.Queries;

namespace Sample.Documents.Api.Controllers
{
    /// <summary>
    /// Controller for lock resource
    /// </summary>
    [RoutePrefix("api/documents/{documentId}/lock")]
    public class LocksController : SecuredApiController
    {
        private readonly ICommand<LockInfo> _putLockCmd;
        private readonly ICommand<LockInfo> _removeLockCmd;

        public LocksController(
            IUserNameQuery userQuery,
            ICommand<LockInfo> putLockCmd,
            ICommand<LockInfo> removeLockCmd)
            : base(userQuery)
        {
            _putLockCmd = putLockCmd;
            _removeLockCmd = removeLockCmd;
        }

        [Route("")]
        public IHttpActionResult Put(Guid documentId)
        {
            return InvokeWhenUserExists(userName => 
            {
                try
                {
                    _putLockCmd.Execute(Envelop(new LockInfo(documentId), userName));
                }
                catch(DocumentLockedException e)
                {
                    return this.ResponseMessage(new HttpResponseMessage(HttpStatusCode.PreconditionFailed) 
                    {
                        Content = new StringContent(e.Message)
                    });
                }

                return this.Ok();
            });
        }

        [Route("")]
        public IHttpActionResult Delete(Guid documentId)
        {
            return InvokeWhenUserExists(userName =>
            {
                try
                {
                    _removeLockCmd.Execute(Envelop(new LockInfo(documentId), userName));
                }
                catch (DocumentLockedException e)
                {
                    return this.ResponseMessage(new HttpResponseMessage(HttpStatusCode.PreconditionFailed)
                    {
                        Content = new StringContent(e.Message)
                    });
                }

                return this.ResponseMessage(new HttpResponseMessage(HttpStatusCode.NoContent));
            });
        }
    }
}
