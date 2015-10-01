using System;

namespace Sample.Tickets.Api.Commands
{
    public interface ITicketReference
    {
        Guid TicketId { get; }
        ulong ExpectedVersion { get; }
    }

    public class Ticket : ITicketReference
    {
        public Guid TicketId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string AssignedTo { get; set; }
        public string Severity { get; set; }
        public string Status { get; set; }
        public ulong ExpectedVersion { get; set; }
    }
}
