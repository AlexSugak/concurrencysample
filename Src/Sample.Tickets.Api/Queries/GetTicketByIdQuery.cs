using Sample.Tickets.Api.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Tickets.Api.Queries
{
    public interface IGetTicketByIdQuery
    {
        TicketDetails Execute(Guid id);
    }

    public class GetTicketByIdQuerySqlQuery : IGetTicketByIdQuery
    {
        private readonly string _connectionString;
        public GetTicketByIdQuerySqlQuery(string connectionString)
        {
            _connectionString = connectionString;
        }

        public TicketDetails Execute(Guid id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string cmdText = "SELECT * FROM [dbo].[Tickets] WHERE [Id] = @id";
                using (var cmd = new SqlCommand(cmdText, connection))
                {
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            throw new TicketNotFoundException(string.Format("Ticket {0} was not found", id));
                        }

                        reader.Read();
                        return new TicketDetails()
                        {
                            Id = (Guid)reader["Id"],
                            Title = (string)reader["Title"],
                            Description = reader["Description"] as string,
                            Severity = (string)reader["Severity"],
                            Status = (string)reader["Status"],
                            AssignedTo = reader["AssignedTo"] as string,
                            Version = (Guid)reader["Version"]
                        };
                    }
                }
            }
        }
    }
}
