﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Sample.Documents.Api.Commands;
using Sample.Documents.Api.Queries;
using Sample.Documents.Api.Controllers;

namespace Sample.Documents.Api
{
    /// <summary>
    /// Composes api controllers which are the app root components 
    /// </summary>
    public class CompositionRoot : IHttpControllerActivator
    {
        private readonly IGetAllDocumentsQuery _getAllDocuments;
        private readonly ICommand<Document> _submitDocumentCmd;
        private readonly ICommand<Document> _updateDocumentCmd;
        private readonly IUserNameQuery _userNameQuery;
        private readonly ICommand<Lock> _putLockCmd;
        private readonly ICommand<Lock> _removeLockCmd;

        public CompositionRoot(
            IGetAllDocumentsQuery getAllDocuments,
            ICommand<Document> submitDocumentCmd,
            ICommand<Document> updateDocumentCmd,
            IUserNameQuery userNameQuery,
            ICommand<Lock> putLockCmd,
            ICommand<Lock> removeLockCmd)
        {
            _getAllDocuments = getAllDocuments;
            _submitDocumentCmd = submitDocumentCmd;
            _updateDocumentCmd = updateDocumentCmd;
            _userNameQuery = userNameQuery;
            _putLockCmd = putLockCmd;
            _removeLockCmd = removeLockCmd;
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
                return new DocumentsController(_userNameQuery, _getAllDocuments, _submitDocumentCmd, _updateDocumentCmd);
            }

            if (controllerType == typeof(LocksController))
            {
                return new LocksController(_userNameQuery, _putLockCmd, _removeLockCmd);
            }

            throw new NotImplementedException(
                string.Format("controller of type {0} is not supported", controllerType.FullName));
        }
    }
}
