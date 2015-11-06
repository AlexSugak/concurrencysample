using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Api.Shared
{
    /// <summary>
    /// wraps any given query with user permissions check
    /// </summary>
    public class SecuredQuery<TIn, TOut> : IQuery<TIn, TOut>
    {
        private readonly IQuery<TIn, TOut> _inner;

        public SecuredQuery(IQuery<TIn, TOut> inner)
        {
            _inner = inner;
        }

        public TOut Execute(Envelope<TIn> request)
        {
            //TODO: implement proper auth
            if (string.IsNullOrEmpty(request.UserName))
            {
                throw new UnauthorizedAccessException("User name required, was empty");
            }

            return _inner.Execute(request);
        }
    }
}
