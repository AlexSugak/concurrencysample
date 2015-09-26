using Sample.Documents.Api.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Sample.Documents.Api.Controllers
{
    public abstract class SecuredApiController : ApiControllerWithEnvelope
    {
        private readonly IUserNameQuery _userQuery;

        public SecuredApiController(IUserNameQuery userQuery)
        {
            _userQuery = userQuery;
        }

        protected IHttpActionResult InvokeWhenUserExists(Func<string, IHttpActionResult> action)
        {
            var userName = _userQuery.Execute(this.Request);
            if (string.IsNullOrEmpty(userName))
            {
                return this.Unauthorized();
            }
            else
            {
                return action(userName);
            }
        }
    }
}
