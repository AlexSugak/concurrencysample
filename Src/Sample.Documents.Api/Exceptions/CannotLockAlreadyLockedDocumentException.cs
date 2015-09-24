using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Documents.Api.Exceptions
{
    [Serializable]
    public class CannotLockAlreadyLockedDocumentException : Exception
    {
        public CannotLockAlreadyLockedDocumentException() { }
        public CannotLockAlreadyLockedDocumentException(string message) : base(message) { }
        public CannotLockAlreadyLockedDocumentException(string message, Exception inner) : base(message, inner) { }
        protected CannotLockAlreadyLockedDocumentException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
