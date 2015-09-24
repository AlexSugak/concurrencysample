using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Sample.Documents.Api.Commands;
using Sample.Documents.Api.Queries;

namespace Sample.Documents.Api.Controllers
{
    /// <summary>
    /// Controller for documents resource
    /// </summary>
    [RoutePrefix("api/documents")]
    public class DocumentsController : SecuredApiController
    {
        private readonly IGetAllDocumentsQuery _getAllDocuments;
        private readonly ISubmitNewDocumentCommand _submitDocumentCmd;

        public DocumentsController(
            IUserNameQuery userQuery,
            IGetAllDocumentsQuery getAllDocuments,
            ISubmitNewDocumentCommand submitDocumentCmd)
            : base(userQuery)
        {
            _getAllDocuments = getAllDocuments;
            _submitDocumentCmd = submitDocumentCmd;
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

        [Route("")]
        public IHttpActionResult Post(DocumentModel model)
        {
            return InvokeWhenUserExists(userName => 
            {
                var id = Guid.NewGuid();
                try
                {
                    _submitDocumentCmd.Execute(new NewDocument(id, model.Title, model.Content));
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
