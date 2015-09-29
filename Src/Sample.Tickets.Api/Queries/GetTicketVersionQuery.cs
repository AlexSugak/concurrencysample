using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Tickets.Api.Queries
{
    public interface IGetTicketVersionQuery
    {
        ulong Execute(HttpRequestMessage request);
    }

    public class IfMatchHttpHeaderTicketVersionQuery : IGetTicketVersionQuery
    {
        public ulong Execute(HttpRequestMessage request)
        {
            if(request.Headers.IfMatch != null)
            {
                var value = request.Headers.IfMatch.FirstOrDefault();
                if (value != null)
                {
                    return ulong.Parse(value.Tag.TrimStart("\"".ToCharArray()).TrimEnd("\"".ToCharArray()));
                }
            }

            return default(ulong);
        }
    }
}
