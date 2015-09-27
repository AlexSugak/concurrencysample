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

namespace Sample.Documents.Api.Controllers
{
    /// <summary>
    /// Controller for lock resource
    /// </summary>
    [RoutePrefix("api/documents/{documentId}/lock")]
    public class LocksController : SecuredApiController
    {
        private readonly ICommand<Lock> _putLockCmd;
        private readonly ICommand<Lock> _removeLockCmd;

        public LocksController(
            IUserNameQuery userQuery,
            ICommand<Lock> putLockCmd,
            ICommand<Lock> removeLockCmd)
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
                    _putLockCmd.Execute(Envelop(new Lock(documentId), userName));
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
                    _removeLockCmd.Execute(Envelop(new Lock(documentId), userName));
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
