using Sample.Documents.Api.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Sample.Documents.Api.Controllers
{
    public abstract class ApiControllerWithEnvelope : ApiController
    {
        protected Envelope<T> Envelop<T>(T item, string userName)
        {
            return new Envelope<T>(item, userName);
        }
    }
}
