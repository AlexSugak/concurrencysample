using System;
using System.Web.Http;

namespace Sample.Api.Shared
{
    public abstract class ApiControllerWithEnvelope : ApiController
    {
        private readonly IEnvelop _envelop;

        public ApiControllerWithEnvelope(IEnvelop envelop)
        {
            _envelop = envelop;
        }

        protected Envelope<T> Envelop<T>(T item)
        {
            return _envelop.Envelop(this.Request, item);
        }
    }
}
