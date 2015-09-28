using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Tickets.Api.Exceptions
{
    [Serializable]
    public class TicketNotFoundException : Exception
    {
        public TicketNotFoundException() { }
        public TicketNotFoundException(string message) : base(message) { }
        public TicketNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected TicketNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
