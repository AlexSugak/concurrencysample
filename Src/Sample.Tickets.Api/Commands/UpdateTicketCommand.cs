using System;
using System.Data.SqlClient;
using Sample.Api.Shared;
using Sample.Tickets.Api.Exceptions;

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
            System.Threading.Thread.Sleep(1007);

            string cmdText = @" 
DECLARE @t TABLE (id uniqueidentifier);

UPDATE [dbo].[Tickets] 
SET 
[Title] = @title, 
[Description] = @description, 
[Severity] = @severity,
[Status] = @status,
[AssignedTo] = @assignedTo

OUTPUT inserted.Id INTO @t(id) 
WHERE [Id] = @id AND (cast([Version] as bigint)) = @version

IF (SELECT COUNT(*) FROM @t) = 0
BEGIN
    RAISERROR ('error changing row since versions do not match'
        ,16 
        ,1)
END;
";

            try
            {
                base.ExecuteNonQuery(
                    cmdText,
                    new SqlParameter("@id", ticket.Item.TicketId),
                    new SqlParameter("@title", ticket.Item.Title),
                    new SqlParameter("@description", ticket.Item.Description),
                    new SqlParameter("@severity", ticket.Item.Severity),
                    new SqlParameter("@status", ticket.Item.Status),
                    new SqlParameter("@assignedTo", ticket.Item.AssignedTo),
                    new SqlParameter("@version", ticket.Item.ExpectedVersion) { SqlDbType = System.Data.SqlDbType.BigInt });
            }
            catch(SqlException e)
            {
                if(e.Errors[0].Class == 16)
                {
                    throw new OptimisticConcurrencyException("Provided ticket version did not match DB version");
                }
            }
        }
    }
}
