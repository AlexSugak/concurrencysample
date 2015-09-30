using Sample.Api.Shared;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Tickets.Api.Commands
{
    public interface ITicketReference
    {
        Guid TicketId { get; }
        ulong ExpectedVersion { get; }
    }

    public class Ticket : ITicketReference
    {
        public Guid TicketId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string AssignedTo { get; set; }
        public string Severity { get; set; }
        public string Status { get; set; }
        public ulong ExpectedVersion { get; set; }
    }

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
