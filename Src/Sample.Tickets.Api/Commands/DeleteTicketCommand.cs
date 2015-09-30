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


    public class DeleteTicketSqlCommand : SqlOperation, ICommand<TicketReference>
    {
        public DeleteTicketSqlCommand(string connectionString)
            : base(connectionString)
        {
        }

        public void Execute(Envelope<TicketReference> id)
        {
            base.ExecuteNonQuery(
                "DELETE FROM [dbo].[Tickets] WHERE [Id] = @id", 
                new SqlParameter("@id", id.Item.TicketId));
        }
    }
}
