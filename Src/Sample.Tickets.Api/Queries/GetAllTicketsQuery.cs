using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Tickets.Api.Queries
{
    public class TicketDetails
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string AssignedTo { get; set; }
        public string Severity { get; set; }
        public string Status { get; set; }
        public Guid Version { get; set; }
    }

    public interface IGetAllTicketsQuery
    {
        IEnumerable<TicketDetails> Execute();
    }

    public class GetAllTicketsSqlQuery : IGetAllTicketsQuery
    {
        private readonly string _connectionString;
        public GetAllTicketsSqlQuery(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<TicketDetails> Execute()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string cmdText = "SELECT * FROM [dbo].[Tickets]";
                using (var cmd = new SqlCommand(cmdText, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return new TicketDetails()
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
}
