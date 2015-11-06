using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Sample.Api.Shared;
using Sample.Documents.Api.Commands;
using Sample.Documents.Api.Queries;
using Sample.Documents.Api.Exceptions;

namespace Sample.Documents.Api.Controllers
{
    /// <summary>
    /// Controller for documents resource
    /// </summary>
    [RoutePrefix("api/documents")]
    public class DocumentsController : ApiControllerWithEnvelope
    {
        private readonly IQuery<EmptyRequest, IEnumerable<DocumentDetails>> _getAllDocuments;
        private readonly IQuery<Guid, DocumentDetails> _getDocument;
        private readonly ICommand<Document> _submitDocumentCmd;
        private readonly ICommand<Document> _updateDocumentCmd;
        private readonly ICommand<DocumentReference> _deleteDocument;

        public DocumentsController(
            IEnvelop envelop,
            IQuery<EmptyRequest, IEnumerable<DocumentDetails>> getAllDocuments,
            IQuery<Guid, DocumentDetails> getDocument,
            ICommand<Document> submitDocumentCmd,
            ICommand<Document> updateDocumentCmd,
            ICommand<DocumentReference> deleteDocument)
            : base(envelop)
        {
            _getAllDocuments = getAllDocuments;
            _getDocument = getDocument;
            _submitDocumentCmd = submitDocumentCmd;
            _updateDocumentCmd = updateDocumentCmd;
            _deleteDocument = deleteDocument;
        }

        [Route("")]
        public IHttpActionResult Get()
        {
            DocumentResponseModel[] docs;
            try
            {
                docs = ReadDocuments().ToArray();
            }
            catch (UnauthorizedAccessException)
            {
                return this.Unauthorized();
            }

            return this.Ok<DocumentsModel>(
                    new DocumentsModel()
                    {
                        Documents = docs
                    });
        }

        [Route("{documentId}")]
        public IHttpActionResult GetById(Guid documentId)
        {
            DocumentDetails doc;
            try
            {
                doc = _getDocument.Execute(Envelop(documentId));
            }
            catch (UnauthorizedAccessException)
            {
                return this.Unauthorized();
            }
            catch (DocumentNotFoundException)
            {
                return this.NotFound();
            }

            return this.Ok<DocumentResponseModel>(MapDocument(doc));
        }


        [Route("")]
        public IHttpActionResult Post(DocumentModel model)
        {
            var id = Guid.NewGuid();
            try
            {
                _submitDocumentCmd.Execute(Envelop(new Document(id, model.Title, model.Content)));
            }
            catch (UnauthorizedAccessException)
            {
                return this.Unauthorized();
            }
            catch (ValidationException e)
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

        [Route("{documentId}")]
        public IHttpActionResult Put(DocumentModel model, Guid documentId)
        {
            try
            {
                _updateDocumentCmd.Execute(Envelop(new Document(documentId, model.Title, model.Content)));
            }
            catch (UnauthorizedAccessException)
            {
                return this.Unauthorized();
            }
            catch (DocumentLockedException e)
            {
                return this.ResponseMessage(new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent(e.Message)
                });
            }

            var doc = _getDocument.Execute(Envelop(documentId));
            return this.Ok<DocumentResponseModel>(MapDocument(doc));
        }

        [Route("{documentId}")]
        public IHttpActionResult Delete(Guid documentId)
        {
            try
            {
                _deleteDocument.Execute(Envelop(new DocumentReference(documentId)));
            }
            catch (UnauthorizedAccessException)
            {
                return this.Unauthorized();
            }
            catch (DocumentLockedException e)
            {
                return this.ResponseMessage(new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent(e.Message)
                });
            }

            return this.ResponseMessage(new HttpResponseMessage(HttpStatusCode.NoContent));
        }

        private IEnumerable<DocumentResponseModel> ReadDocuments()
        {
            return _getAllDocuments
                        .Execute(Envelop(new EmptyRequest()))
                        .Select(MapDocument);
        }

        private DocumentResponseModel MapDocument(DocumentDetails doc)
        {
            return new DocumentResponseModel()
                        {
                            Id = doc.Id.ToString(),
                            Title = doc.Title,
                            Content = doc.Content,
                            CheckedOutBy = doc.CheckedOutBy
                        };
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
