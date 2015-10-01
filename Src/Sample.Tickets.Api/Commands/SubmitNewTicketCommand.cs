using System;
using System.Data.SqlClient;
using Sample.Api.Shared;

namespace Sample.Tickets.Api.Commands
{
    public class SubmitNewTicketSqlCommand : SqlOperation, ICommand<Ticket>
    {
        public SubmitNewTicketSqlCommand(string connectionString)
            : base(connectionString)
        {
        }

        public void Execute(Envelope<Ticket> ticket)
        {
            string cmdText = @"INSERT INTO [dbo].[Tickets] 
                               ([Id], [Title], [Description], [Severity], [Status], [AssignedTo]) 
                               VALUES 
                               (@id, @title, @description, @severity, @status, @assignedTo)";

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
