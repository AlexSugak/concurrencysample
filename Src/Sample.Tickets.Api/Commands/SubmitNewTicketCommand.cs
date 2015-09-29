using Sample.Api.Shared;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Tickets.Api.Commands
{
    public class Ticket
    {
        public Guid TicketId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string AssignedTo { get; set; }
        public string Severity { get; set; }
        public string Status { get; set; }
    }

    public class SubmitNewTicketSqlCommand : ICommand<Ticket>
    {
        private readonly string _connectionString;

        public SubmitNewTicketSqlCommand(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Execute(Envelope<Ticket> ticket)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"INSERT INTO [dbo].[Tickets] 
                                   ([Id], [Title], [Description], [Severity], [Status], [AssignedTo]) 
                                   VALUES 
                                   (@id, @title, @description, @severity, @status, @assignedTo)";
                using (var cmd = new SqlCommand(cmdText, connection))
                {
                    cmd.Parameters.Add(new SqlParameter("@id", ticket.Item.TicketId));
                    cmd.Parameters.Add(new SqlParameter("@title", ticket.Item.Title));
                    cmd.Parameters.Add(new SqlParameter("@description", ticket.Item.Description));
                    cmd.Parameters.Add(new SqlParameter("@severity", ticket.Item.Severity));
                    cmd.Parameters.Add(new SqlParameter("@status", ticket.Item.Status));
                    cmd.Parameters.Add(new SqlParameter("@assignedTo", ticket.Item.AssignedTo));

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
