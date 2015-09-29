using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Tickets.Api.Exceptions
{
    [Serializable]
    public class OptimisticConcurrencyException : Exception
    {
        public OptimisticConcurrencyException() { }
        public OptimisticConcurrencyException(string message) : base(message) { }
        public OptimisticConcurrencyException(string message, Exception inner) : base(message, inner) { }
        protected OptimisticConcurrencyException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
