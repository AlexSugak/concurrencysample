using System;
using System.Data.SqlClient;
using Sample.Api.Shared;
using Sample.Documents.Api.Exceptions;
using Sample.Documents.Api.Queries;

namespace Sample.Documents.Api.Commands
{
    /// <summary>
    /// Checks if document is locked before executing provided document-related command 
    /// </summary>
    public class DocumentLockValidator<T> : ICommand<T>
        where T : IDocumentReference
    {
        private readonly ICommand<T> _implementation;
        private readonly IGetDocumentQuery _docQuery;

        public DocumentLockValidator(ICommand<T> implementation, IGetDocumentQuery docQuery)
        {
            _implementation = implementation;
            _docQuery = docQuery;
        }

        public void Execute(Envelope<T> lockInfo)
        {
            var doc = _docQuery.Execute(lockInfo.Item.DocumentId);

            if (!string.IsNullOrEmpty(doc.CheckedOutBy) && doc.CheckedOutBy != lockInfo.UserName)
            {
                throw new DocumentLockedException(
                            string.Format(
                                    "Document '{0}' is locked by user '{1}'",
                                    lockInfo.Item.DocumentId,
                                    doc.CheckedOutBy));
            }

            _implementation.Execute(lockInfo);
        }
    }
}
