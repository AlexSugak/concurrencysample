using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Sample.Api.Shared;

namespace Sample.Tickets.Api.Queries
{
    public class IfMatchHttpHeaderTicketVersionQuery : IQuery<HttpRequestMessage, ulong>
    {
        public ulong Execute(Envelope<HttpRequestMessage> request)
        {
            if(request.Item.Headers.IfMatch != null)
            {
                var value = request.Item.Headers.IfMatch.FirstOrDefault();
                if (value != null)
                {
                    return ulong.Parse(value.Tag.TrimStart("\"".ToCharArray()).TrimEnd("\"".ToCharArray()));
                }
            }

            return default(ulong);
        }
    }
}
