using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Documents.Api.Exceptions
{
    [Serializable]
    public class DocumentLockedException : Exception
    {
        public DocumentLockedException() { }
        public DocumentLockedException(string message) : base(message) { }
        public DocumentLockedException(string message, Exception inner) : base(message, inner) { }
        protected DocumentLockedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
