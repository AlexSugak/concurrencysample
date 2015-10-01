using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Sample.Api.Shared;

namespace Sample.Tickets.Api.Queries
{
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
            string cmdText = "SELECT * FROM [dbo].[Tickets] ORDER BY [Title]";
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
