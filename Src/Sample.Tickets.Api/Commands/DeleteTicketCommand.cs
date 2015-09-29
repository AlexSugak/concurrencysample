using Sample.Api.Shared;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Tickets.Api.Commands
{
    public class TicketReference : ITicketReference
    {
        public TicketReference(Guid id, ulong expectedVersion)
        {
            TicketId = id;
            ExpectedVersion = expectedVersion;
        }

        public Guid TicketId { get; private set; }
        public ulong ExpectedVersion { get; private set; }
    }


    public class DeleteTicketSqlCommand : ICommand<TicketReference>
    {
        private readonly string _connectionString;

        public DeleteTicketSqlCommand(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Execute(Envelope<TicketReference> id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    string cmdText = "DELETE FROM [dbo].[Tickets] WHERE [Id] = @id";
                    using (var cmd = new SqlCommand(cmdText, transaction.Connection, transaction))
                    {
                        cmd.Parameters.Add(new SqlParameter("@id", id.Item.TicketId));
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
        }
    }
}
