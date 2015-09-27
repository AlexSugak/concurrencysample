using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Api.Shared
{
    /// <summary>
    /// Wraps any object into envelope with additional metadata (e.g. user name)
    /// </summary>
    public class Envelope<T>
    {
        public Envelope(T item, string userName)
        {
            Item = item;
            UserName = userName;
        }

        public T Item { get; private set; }
        public string UserName { get; private set; }
    }
}
