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
        private readonly IQuery<Guid, DocumentDetails> _docQuery;

        public DocumentLockValidator(ICommand<T> implementation, IQuery<Guid, DocumentDetails> docQuery)
        {
            _implementation = implementation;
            _docQuery = docQuery;
        }

        public void Execute(Envelope<T> lockInfo)
        {
            var doc = _docQuery.Execute(lockInfo.Envelop(lockInfo.Item.DocumentId));

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

    public static class LockValidatorExtention
    {
        public static ICommand<T> WithLockValidation<T>(this ICommand<T> cmd, IQuery<Guid, DocumentDetails> getDocQuery)
            where T : IDocumentReference
        {
            return new DocumentLockValidator<T>(cmd, getDocQuery);
        }
    }
}
