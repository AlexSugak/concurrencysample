using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Documents.Api.Exceptions
{
    [Serializable]
    public class CannotRemoveAnotherUsersLockException : Exception
    {
        public CannotRemoveAnotherUsersLockException() { }
        public CannotRemoveAnotherUsersLockException(string message) : base(message) { }
        public CannotRemoveAnotherUsersLockException(string message, Exception inner) : base(message, inner) { }
        protected CannotRemoveAnotherUsersLockException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
