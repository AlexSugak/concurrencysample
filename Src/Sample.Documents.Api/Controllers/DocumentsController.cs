using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Sample.Documents.Api.Commands;
using Sample.Documents.Api.Queries;

namespace Sample.Documents.Api
{
    /// <summary>
    /// Controller for documents resource
    /// </summary>
    [RoutePrefix("api/documents")]
    public class DocumentsController : ApiController
    {
        private readonly IGetAllDocumentsQuery _getAllDocuments;
        private readonly ISubmitNewDocumentCommand _submitDocumentCmd;

        public DocumentsController(
            IGetAllDocumentsQuery getAllDocuments,
            ISubmitNewDocumentCommand submitDocumentCmd)
        {
            _getAllDocuments = getAllDocuments;
            _submitDocumentCmd = submitDocumentCmd;
        }

        [Route("")]
        public IHttpActionResult Get()
        {
            return this.Ok<DocumentsModel>(new DocumentsModel()
            {
                Documents = ReadDocuments().ToArray()
            });
        }

        [Route("")]
        public IHttpActionResult Post(DocumentModel model)
        {
            var id = Guid.NewGuid();
            try
            {
                WriteDocument(id, model);
            }
            catch(ValidationException e)
            {
                return this.BadRequest(e.Message);
            }

            return this.Created<DocumentResponseModel>("", new DocumentResponseModel() 
            {
                Id = id.ToString(),
                Title = model.Title,
                Content = model.Content
            });
        }

        private void WriteDocument(Guid id, DocumentModel model)
        {
            _submitDocumentCmd.Execute(new NewDocument(id, model.Title, model.Content));
        }

        private IEnumerable<DocumentResponseModel> ReadDocuments()
        {
            return _getAllDocuments
                        .Execute()
                        .Select(d => new DocumentResponseModel()
                        {
                            Id = d.Id.ToString(),
                            Title = d.Title,
                            Content = d.Content,
                            CheckedOutBy = d.CheckedOutBy
                        });
        }
    }

    public class DocumentsModel
    {
        public DocumentResponseModel[] Documents { get; set; }
    }

    public class DocumentModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
    }

    public class DocumentResponseModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string CheckedOutBy { get; set; }
    }
}
