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
    public class LockCommandValidator<T> : ICommand<T>
        where T : IDocumentReference
    {
        private readonly ICommand<T> _implementation;
        private readonly IGetDocumentQuery _docQuery;

        public LockCommandValidator(ICommand<T> implementation, IGetDocumentQuery docQuery)
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

    public class PutLockOnDocumentSqlCommand : ICommand<Lock>
    {
        private readonly string _connectionString;

        public PutLockOnDocumentSqlCommand(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Execute(Envelope<Lock> lockInfo)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = "UPDATE [dbo].[Documents] SET [CheckedOutBy] = @user WHERE [Id] = @id";
                using (var cmd = new SqlCommand(cmdText, connection))
                {
                    cmd.Parameters.Add(new SqlParameter("@id", lockInfo.Item.DocumentId));
                    cmd.Parameters.Add(new SqlParameter("@user", lockInfo.UserName));

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
