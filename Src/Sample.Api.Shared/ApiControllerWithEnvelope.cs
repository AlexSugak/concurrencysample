using System;
using System.Web.Http;

namespace Sample.Api.Shared
{
    public abstract class ApiControllerWithEnvelope : ApiController
    {
        protected Envelope<T> Envelop<T>(T item, string userName)
        {
            return new Envelope<T>(item, userName);
        }
    }
}
