using System;
using System.Data.SqlClient;
using Sample.Api.Shared;
using Sample.Tickets.Api.Exceptions;

namespace Sample.Tickets.Api.Commands
{
    public class DeleteTicketSqlCommand : SqlOperation, ICommand<TicketReference>
    {
        public DeleteTicketSqlCommand(string connectionString)
            : base(connectionString)
        {
        }

        public void Execute(Envelope<TicketReference> id)
        {
            try
            {
                base.ExecuteNonQuery(
                    @"
DECLARE @t TABLE (id uniqueidentifier);

DELETE FROM [dbo].[Tickets] 
OUTPUT deleted.Id INTO @t(id) 
WHERE [Id] = @id AND (cast([Version] as bigint)) = @version

IF (SELECT COUNT(*) FROM @t) = 0
BEGIN
    RAISERROR ('error deleting row since versions do not match'
        ,16 
        ,1)
END;"
                    ,
                    new SqlParameter("@id", id.Item.TicketId),
                    new SqlParameter("@version", id.Item.ExpectedVersion) { SqlDbType = System.Data.SqlDbType.BigInt });

            }
            catch(SqlException e)
            {
                if (e.Errors[0].Class == 16)
                {
                    throw new OptimisticConcurrencyException("Provided ticket version did not match DB version");
                }
            }
        }
    }
}
