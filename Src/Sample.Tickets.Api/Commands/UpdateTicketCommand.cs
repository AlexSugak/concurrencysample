using System;
using System.Data.SqlClient;
using Sample.Api.Shared;

namespace Sample.Tickets.Api.Commands
{
    public class UpdateTicketSqlCommand : SqlOperation, ICommand<Ticket>
    {
        public UpdateTicketSqlCommand(string connectionString)
            : base(connectionString)
        {
        }

        public void Execute(Envelope<Ticket> ticket)
        {
            string cmdText = @" UPDATE [dbo].[Tickets] 
                                SET 
                                [Title] = @title, 
                                [Description] = @description, 
                                [Severity] = @severity,
                                [Status] = @status,
                                [AssignedTo] = @assignedTo
                                WHERE [Id] = @id";

            base.ExecuteNonQuery(
                cmdText,
                new SqlParameter("@id", ticket.Item.TicketId),
                new SqlParameter("@title", ticket.Item.Title),
                new SqlParameter("@description", ticket.Item.Description),
                new SqlParameter("@severity", ticket.Item.Severity),
                new SqlParameter("@status", ticket.Item.Status),
                new SqlParameter("@assignedTo", ticket.Item.AssignedTo));
        }
    }
}
