using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

        public Envelope<TOther> Envelop<TOther>(TOther other)
        {
            return new Envelope<TOther>(other, this.UserName);
        }
    }

    public interface IEnvelop
    {
        Envelope<T> Envelop<T>(HttpRequestMessage request, T item);
    }

    public class EnvelopeWithUserName : IEnvelop
    {
        private readonly IUserNameQuery _userQuery;

        public EnvelopeWithUserName(IUserNameQuery userQuery)
        {
            _userQuery = userQuery;
        }

        public Envelope<T> Envelop<T>(HttpRequestMessage request, T item)
        {
            return new Envelope<T>(item, _userQuery.Execute(request));
        }
    }
}
