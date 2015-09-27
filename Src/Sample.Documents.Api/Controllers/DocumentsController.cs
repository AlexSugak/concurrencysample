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
    public class DocumentsController : SecuredApiController
    {
        private readonly IGetAllDocumentsQuery _getAllDocuments;
        private readonly IGetDocumentQuery _getDocument;
        private readonly ICommand<Document> _submitDocumentCmd;
        private readonly ICommand<Document> _updateDocumentCmd;
        private readonly ICommand<DocumentReference> _deleteDocument;

        public DocumentsController(
            IUserNameQuery userQuery,
            IGetAllDocumentsQuery getAllDocuments,
            IGetDocumentQuery getDocument,
            ICommand<Document> submitDocumentCmd,
            ICommand<Document> updateDocumentCmd,
            ICommand<DocumentReference> deleteDocument)
            : base(userQuery)
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
            return InvokeWhenUserExists(userName => this.Ok<DocumentsModel>(
                                                            new DocumentsModel()
                                                            {
                                                                Documents = ReadDocuments().ToArray()
                                                            }));
        }

        [Route("{documentId}")]
        public IHttpActionResult GetById(Guid documentId)
        {
            return InvokeWhenUserExists(userName =>
            {
                DocumentDetails doc;
                try
                {
                    doc = _getDocument.Execute(documentId);
                }
                catch(DocumentNotFoundException)
                {
                    return this.NotFound();
                }

                return this.Ok<DocumentResponseModel>(
                                                        new DocumentResponseModel()
                                                        {
                                                            Id = doc.Id.ToString(),
                                                            Title = doc.Title,
                                                            Content = doc.Content,
                                                            CheckedOutBy = doc.CheckedOutBy
                                                        });
            });
        }
            

        [Route("")]
        public IHttpActionResult Post(DocumentModel model)
        {
            return InvokeWhenUserExists(userName => 
            {
                var id = Guid.NewGuid();
                try
                {
                    _submitDocumentCmd.Execute(Envelop(new Document(id, model.Title, model.Content), userName));
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
            });
        }

        [Route("{documentId}")]
        public IHttpActionResult Put(DocumentModel model, Guid documentId)
        {
            return InvokeWhenUserExists(userName =>
            {
                try
                {
                    _updateDocumentCmd.Execute(Envelop(new Document(documentId, model.Title, model.Content), userName));
                }
                catch(DocumentLockedException e)
                {
                    return this.ResponseMessage(new HttpResponseMessage(HttpStatusCode.Conflict) 
                    { 
                        Content = new StringContent(e.Message) 
                    });
                }

                return this.Ok<DocumentResponseModel>(new DocumentResponseModel()
                {
                    Id = documentId.ToString(),
                    Title = model.Title,
                    Content = model.Content,
                    CheckedOutBy = userName
                });
            });
        }

        [Route("{documentId}")]
        public IHttpActionResult Delete(Guid documentId)
        {
            return InvokeWhenUserExists(userName =>
            {
                try
                {
                    _deleteDocument.Execute(Envelop(new DocumentReference(documentId), userName));
                }
                catch(DocumentLockedException e)
                {
                    return this.ResponseMessage(new HttpResponseMessage(HttpStatusCode.Conflict)
                    {
                        Content = new StringContent(e.Message)
                    });
                }

                return this.ResponseMessage(new HttpResponseMessage(HttpStatusCode.NoContent));
            });
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
