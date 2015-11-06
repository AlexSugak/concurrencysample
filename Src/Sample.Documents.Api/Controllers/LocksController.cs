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
    public class LocksController : ApiControllerWithEnvelope
    {
        private readonly ICommand<LockInfo> _putLockCmd;
        private readonly ICommand<LockInfo> _removeLockCmd;

        public LocksController(
            IEnvelop envelop,
            ICommand<LockInfo> putLockCmd,
            ICommand<LockInfo> removeLockCmd)
            : base(envelop)
        {
            _putLockCmd = putLockCmd;
            _removeLockCmd = removeLockCmd;
        }

        [Route("")]
        public IHttpActionResult Put(Guid documentId)
        {
            try
            {
                _putLockCmd.Execute(Envelop(new LockInfo(documentId)));
            }
            catch(UnauthorizedAccessException)
            {
                return this.Unauthorized();
            }
            catch (DocumentLockedException e)
            {
                return this.ResponseMessage(new HttpResponseMessage(HttpStatusCode.PreconditionFailed)
                {
                    Content = new StringContent(e.Message)
                });
            }

            return this.Ok();
        }

        [Route("")]
        public IHttpActionResult Delete(Guid documentId)
        {
            try
            {
                _removeLockCmd.Execute(Envelop(new LockInfo(documentId)));
            }
            catch (UnauthorizedAccessException)
            {
                return this.Unauthorized();
            }
            catch (DocumentLockedException e)
            {
                return this.ResponseMessage(new HttpResponseMessage(HttpStatusCode.PreconditionFailed)
                {
                    Content = new StringContent(e.Message)
                });
            }

            return this.ResponseMessage(new HttpResponseMessage(HttpStatusCode.NoContent));
        }
    }
}
