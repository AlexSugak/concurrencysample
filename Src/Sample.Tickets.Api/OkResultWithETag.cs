using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace Sample.Tickets.Api
{
    /// <summary>
    /// Represents ok result with ETag header
    /// </summary>
    public class OkResultWithETag<T> : OkNegotiatedContentResult<T>
    {
        public OkResultWithETag(T content, ApiController controller)
            : base(content, controller) { }

        public OkResultWithETag(T content, IContentNegotiator contentNegotiator, HttpRequestMessage request, IEnumerable<MediaTypeFormatter> formatters)
            : base(content, contentNegotiator, request, formatters) { }

        public string ETagValue { get; set; }

        public override async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await base.ExecuteAsync(cancellationToken);

            try
            {
                response.Headers.ETag = new EntityTagHeaderValue("\"" + this.ETagValue + "\"");
            }
            catch(Exception e)
            {
                throw e;
            }

            return response;
        }
    }
}
