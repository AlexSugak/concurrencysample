using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Documents.Api.Exceptions
{
    [Serializable]
    public class DocumentNotFoundException : Exception
    {
        public DocumentNotFoundException() { }
        public DocumentNotFoundException(string message) : base(message) { }
        public DocumentNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected DocumentNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
