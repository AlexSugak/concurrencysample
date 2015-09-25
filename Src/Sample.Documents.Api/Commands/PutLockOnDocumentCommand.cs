using Sample.Documents.Api.Exceptions;
using Sample.Documents.Api.Queries;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Documents.Api.Commands
{
    public interface IPutLockOnDocumentCommand
    {
        void Execute(string userName, Guid documentId);
    }

    public class PutLockCommandValidator : IPutLockOnDocumentCommand
    {
        private readonly IPutLockOnDocumentCommand _implementation;
        private readonly IGetDocumentQuery _docQuery;

        public PutLockCommandValidator(IPutLockOnDocumentCommand implementation, IGetDocumentQuery docQuery)
        {
            _implementation = implementation;
            _docQuery = docQuery;
        }

        public void Execute(string userName, Guid documentId)
        {
            var doc = _docQuery.Execute(documentId);

            if(doc.CheckedOutBy == userName)
            {
                return;
            }

            if(!string.IsNullOrEmpty(doc.CheckedOutBy) && doc.CheckedOutBy != userName)
            {
                throw new CannotLockAlreadyLockedDocumentException(
                            string.Format(
                                    "Cannot put a lock on document {0} for user {1} because it is already locked by user {2}",
                                    documentId,
                                    userName,
                                    doc.CheckedOutBy));
            }

            _implementation.Execute(userName, documentId);
        }
    }

    public class PutLockOnDocumentSqlCommand : IPutLockOnDocumentCommand
    {
        private readonly string _connectionString;

        public PutLockOnDocumentSqlCommand(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Execute(string userName, Guid documentId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    string cmdText = "UPDATE [dbo].[Documents] SET [CheckedOutBy] = @user WHERE [Id] = @id";
                    using (var cmd = new SqlCommand(cmdText, transaction.Connection, transaction))
                    {
                        cmd.Parameters.Add(new SqlParameter("@id", documentId));
                        cmd.Parameters.Add(new SqlParameter("@user", userName));

                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
        }
    }
}
