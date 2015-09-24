using Sample.Documents.Api.Queries;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Sample.Documents.Api
{
    [RoutePrefix("api/documents/{documentId}/lock")]
    public class LocksController : ApiController
    {
        private readonly string _connectionString;
        private readonly IUserNameQuery _userQuery;

        public LocksController(IUserNameQuery userQuery)
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DocumentsDBConnectionString"].ConnectionString;
            _userQuery = userQuery;
        }

        [Route("")]
        public IHttpActionResult Put(Guid documentId)
        {
            var userName = _userQuery.Execute(this.Request);

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

            return this.Ok();
        }

        [Route("")]
        public IHttpActionResult Delete(Guid documentId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    string cmdText = "UPDATE [dbo].[Documents] SET [CheckedOutBy] = null WHERE [Id] = @id";
                    using (var cmd = new SqlCommand(cmdText, transaction.Connection, transaction))
                    {
                        cmd.Parameters.Add(new SqlParameter("@id", documentId));
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }

            return this.Ok();
        }
    }
}
