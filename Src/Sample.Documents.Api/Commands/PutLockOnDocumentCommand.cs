using System;
using System.Data.SqlClient;
using Sample.Api.Shared;
using Sample.Documents.Api.Exceptions;
using Sample.Documents.Api.Queries;

namespace Sample.Documents.Api.Commands
{
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

    public class PutLockOnDocumentSqlCommand : SqlOperation, ICommand<LockInfo>
    {
        public PutLockOnDocumentSqlCommand(string connectionString)
            : base(connectionString)
        {
        }

        public void Execute(Envelope<LockInfo> lockInfo)
        {
            base.ExecuteNonQuery(
                "UPDATE [dbo].[Documents] SET [CheckedOutBy] = @user WHERE [Id] = @id",
                new SqlParameter("@id", lockInfo.Item.DocumentId),
                new SqlParameter("@user", lockInfo.UserName));
        }
    }
}
