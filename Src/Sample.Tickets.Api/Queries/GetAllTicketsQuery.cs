using Sample.Api.Shared;
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
        public ulong Version { get; set; }
    }

    public interface IGetAllTicketsQuery
    {
        IEnumerable<TicketDetails> Execute();
    }

    public class GetAllTicketsSqlQuery : SqlOperation, IGetAllTicketsQuery
    {
        public GetAllTicketsSqlQuery(string connectionString)
            : base(connectionString)
        {
        }

        public IEnumerable<TicketDetails> Execute()
        {
            string cmdText = "SELECT * FROM [dbo].[Tickets]";
            return base.ExecuteReader<TicketDetails>(
                cmdText,
                reader => new TicketDetails()
                            {
                                Id = (Guid)reader["Id"],
                                Title = (string)reader["Title"],
                                Description = reader["Description"] as string,
                                Severity = (string)reader["Severity"],
                                Status = (string)reader["Status"],
                                AssignedTo = reader["AssignedTo"] as string,
                                Version = BitConverter.ToUInt64(((byte[])reader["Version"]).Reverse().ToArray(), 0)
                            });
        }
    }
}
