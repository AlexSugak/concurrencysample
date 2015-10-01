using System;

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
}
