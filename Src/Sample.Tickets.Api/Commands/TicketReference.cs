using System;

namespace Sample.Tickets.Api.Commands
{
    public class TicketReference : ITicketReference
    {
        public TicketReference(Guid id, ulong expectedVersion)
        {
            TicketId = id;
            ExpectedVersion = expectedVersion;
        }

        public Guid TicketId { get; private set; }
        public ulong ExpectedVersion { get; private set; }
    }
}
