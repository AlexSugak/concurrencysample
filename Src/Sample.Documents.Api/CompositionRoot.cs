using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Sample.Documents.Api.Commands;
using Sample.Documents.Api.Queries;

namespace Sample.Documents.Api
{
    /// <summary>
    /// Composes api controllers which are the app root components 
    /// </summary>
    public class CompositionRoot : IHttpControllerActivator
    {
        private readonly IGetAllDocumentsQuery _getAllDocuments;
        private readonly ISubmitNewDocumentCommand _submitDocumentCmd;
        private readonly IUserNameQuery _userNameQuery;

        public CompositionRoot(
            IGetAllDocumentsQuery getAllDocuments,
            ISubmitNewDocumentCommand submitDocumentCmd,
            IUserNameQuery userNameQuery)
        {
            _getAllDocuments = getAllDocuments;
            _submitDocumentCmd = submitDocumentCmd;
            _userNameQuery = userNameQuery;
        }

        public IHttpController Create(
            HttpRequestMessage request, 
            HttpControllerDescriptor controllerDescriptor, 
            Type controllerType)
        {
            if(controllerType == typeof(HomeController))
            {
                return new HomeController();
            }

            if(controllerType == typeof(DocumentsController))
            {
                return new DocumentsController(_getAllDocuments, _submitDocumentCmd);
            }

            if (controllerType == typeof(LocksController))
            {
                return new LocksController(_userNameQuery);
            }

            throw new NotImplementedException(
                string.Format("controller of type {0} is not supported", controllerType.FullName));
        }
    }
}
