using System;
using System.Data.SqlClient;
using Sample.Api.Shared;

namespace Sample.Tickets.Api.Commands
{
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
