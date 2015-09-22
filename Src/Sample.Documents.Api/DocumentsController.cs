using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Sample.Documents.Api
{
    [RoutePrefix("api/documents")]
    public class DocumentsController : ApiController
    {
        private static ConcurrentBag<DocumentModel> _items = new ConcurrentBag<DocumentModel>();

        [Route("")]
        public HttpResponseMessage Get()
        {
            return this.Request.CreateResponse<DocumentsModel>(new DocumentsModel() { Documents = _items.ToArray() });
        }

        [Route("")]
        public HttpResponseMessage Post(DocumentModel model)
        {
            _items.Add(model);
            return this.Request.CreateResponse(HttpStatusCode.Created);
        }
    }

    public class DocumentsModel
    {
        public DocumentModel[] Documents { get; set; }
    }

    public class DocumentModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
