using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Sample.Api.Shared;
using Sample.Tickets.Api.Exceptions;

namespace Sample.Tickets.Api.Queries
{
    public interface IGetTicketByIdQuery
    {
        TicketDetails Execute(Guid id);
    }

    public class GetTicketByIdQuerySqlQuery : SqlOperation, IGetTicketByIdQuery
    {
        public GetTicketByIdQuerySqlQuery(string connectionString)
            : base(connectionString)
        {
        }

        public TicketDetails Execute(Guid id)
        {
            return base.ExecuteReaderOnce<TicketDetails>(
                "SELECT * FROM [dbo].[Tickets] WHERE [Id] = @id",
                reader => new TicketDetails()
                            {
                                Id = (Guid)reader["Id"],
                                Title = (string)reader["Title"],
                                Description = reader["Description"] as string,
                                Severity = (string)reader["Severity"],
                                Status = (string)reader["Status"],
                                AssignedTo = reader["AssignedTo"] as string,
                                Version = BitConverter.ToUInt64(((byte[])reader["Version"]).Reverse().ToArray(), 0)
                            },
                () => new TicketNotFoundException(string.Format("Ticket {0} was not found", id)),
                new SqlParameter("@id", id));
        }
    }
}
