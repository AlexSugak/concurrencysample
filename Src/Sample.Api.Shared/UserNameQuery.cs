using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Sample.Api.Shared
{
    /// <summary>
    /// Gets user name from http request
    /// </summary>
    public interface IUserNameQuery
    {
        string Execute(HttpRequestMessage request);
    }

    /// <summary>
    /// Gets user name by parsing Bearer header into <see cref="SimpleToken"/>
    /// </summary>
    public class SimppleTokenUserNameQuery : IUserNameQuery
    {
        public string Execute(HttpRequestMessage request)
        {
            if (request.Headers.Authorization == null)
            {
                return null;
            }

            if (request.Headers.Authorization.Scheme != "Bearer")
            {
                return null;
            }

            var header = request.Headers.Authorization.Parameter;

            SimpleToken token;
            if (SimpleToken.TryParse(header, out token))
            {
                var claim = token.FirstOrDefault(t => t.Type == "userName");
                if (claim != null)
                {
                    return claim.Value;
                }
            }

            return null;
        }
    }
}
