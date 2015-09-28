using Sample.Api.Shared;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Tickets.Api.Commands
{
    public class UpdateTicketSqlCommand : ICommand<Ticket>
    {
        private readonly string _connectionString;

        public UpdateTicketSqlCommand(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Execute(Envelope<Ticket> ticket)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"UPDATE [dbo].[Tickets] 
                                   SET 
                                    [Title] = @title, 
                                    [Description] = @description, 
                                    [Severity] = @severity,
                                    [Status] = @status,
                                    [AssignedTo] = @assignedTo,
                                    [Version] = @version
                                   WHERE [Id] = @id";
                using (var cmd = new SqlCommand(cmdText, connection))
                {
                    cmd.Parameters.Add(new SqlParameter("@id", ticket.Item.TicketId));
                    cmd.Parameters.Add(new SqlParameter("@title", ticket.Item.Title));
                    cmd.Parameters.Add(new SqlParameter("@description", ticket.Item.Description));
                    cmd.Parameters.Add(new SqlParameter("@severity", ticket.Item.Severity));
                    cmd.Parameters.Add(new SqlParameter("@status", ticket.Item.Status));
                    cmd.Parameters.Add(new SqlParameter("@assignedTo", ticket.Item.AssignedTo));
                    cmd.Parameters.Add(new SqlParameter("@version", ticket.Item.Version));

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
