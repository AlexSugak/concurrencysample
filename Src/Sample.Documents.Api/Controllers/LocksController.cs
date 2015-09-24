using Sample.Documents.Api.Commands;
using Sample.Documents.Api.Exceptions;
using Sample.Documents.Api.Queries;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Sample.Documents.Api
{
    [RoutePrefix("api/documents/{documentId}/lock")]
    public class LocksController : ApiController
    {
        private readonly IUserNameQuery _userQuery;
        private readonly IPutLockOnDocumentCommand _putLockCmd;
        private readonly IRemoveLockFromDocumentCommand _removeLockCmd;

        public LocksController(
            IUserNameQuery userQuery,
            IPutLockOnDocumentCommand putLockCmd,
            IRemoveLockFromDocumentCommand removeLockCmd)
        {
            _userQuery = userQuery;
            _putLockCmd = putLockCmd;
            _removeLockCmd = removeLockCmd;
        }

        [Route("")]
        public IHttpActionResult Put(Guid documentId)
        {
            var userName = _userQuery.Execute(this.Request);
            if(string.IsNullOrEmpty(userName))
            {
                return this.Unauthorized();
            }

            try
            {
                _putLockCmd.Execute(userName, documentId);
            }
            catch(CannotLockAlreadyLockedDocumentException)
            {
                return this.ResponseMessage(new HttpResponseMessage(HttpStatusCode.PreconditionFailed));
            }

            return this.Ok();
        }

        [Route("")]
        public IHttpActionResult Delete(Guid documentId)
        {
            var userName = _userQuery.Execute(this.Request);
            if (string.IsNullOrEmpty(userName))
            {
                return this.Unauthorized();
            }

            try
            {
                _removeLockCmd.Execute(userName, documentId);
            }
            catch(CannotRemoveAnotherUsersLockException)
            {
                return this.ResponseMessage(new HttpResponseMessage(HttpStatusCode.PreconditionFailed));
            }

            return this.ResponseMessage(new HttpResponseMessage(HttpStatusCode.NoContent));
        }
    }
}
