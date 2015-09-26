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
    public class PutLockCommandValidator : ICommand<Lock>
    {
        private readonly ICommand<Lock> _implementation;
        private readonly IGetDocumentQuery _docQuery;

        public PutLockCommandValidator(ICommand<Lock> implementation, IGetDocumentQuery docQuery)
        {
            _implementation = implementation;
            _docQuery = docQuery;
        }

        public void Execute(Lock lockInfo)
        {
            var doc = _docQuery.Execute(lockInfo.DocumentId);

            if(doc.CheckedOutBy == lockInfo.UserName)
            {
                return;
            }

            if (!string.IsNullOrEmpty(doc.CheckedOutBy) && doc.CheckedOutBy != lockInfo.UserName)
            {
                throw new CannotLockAlreadyLockedDocumentException(
                            string.Format(
                                    "Cannot put a lock on document {0} for user {1} because it is already locked by user {2}",
                                    lockInfo.DocumentId,
                                    lockInfo.UserName,
                                    doc.CheckedOutBy));
            }

            _implementation.Execute(lockInfo);
        }
    }

    public class PutLockOnDocumentSqlCommand : ICommand<Lock>
    {
        private readonly string _connectionString;

        public PutLockOnDocumentSqlCommand(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Execute(Lock lockInfo)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    string cmdText = "UPDATE [dbo].[Documents] SET [CheckedOutBy] = @user WHERE [Id] = @id";
                    using (var cmd = new SqlCommand(cmdText, transaction.Connection, transaction))
                    {
                        cmd.Parameters.Add(new SqlParameter("@id", lockInfo.DocumentId));
                        cmd.Parameters.Add(new SqlParameter("@user", lockInfo.UserName));

                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
        }
    }
}
