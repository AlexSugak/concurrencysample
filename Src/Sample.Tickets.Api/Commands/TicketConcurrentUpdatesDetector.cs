using System;
using Sample.Api.Shared;
using Sample.Tickets.Api.Exceptions;
using Sample.Tickets.Api.Queries;

namespace Sample.Tickets.Api.Commands
{
    /// <summary>
    /// Checks that expected ticket version matches DB version before executing provided command
    /// </summary>
    [Obsolete("Version checking moved to DB layer.")]
    public class TicketConcurrentUpdatesDetector<T> : ICommand<T> where T : ITicketReference
    {
        private readonly ICommand<T> _implementation;
        private readonly IQuery<Guid, TicketDetails> _getTicketQuery;

        public TicketConcurrentUpdatesDetector(
            ICommand<T> implementation,
            IQuery<Guid, TicketDetails> getTicketQuery)
        {
            _implementation = implementation;
            _getTicketQuery = getTicketQuery;
        }

        public void Execute(Envelope<T> input)
        {
            var existing = _getTicketQuery.Execute(input.Envelop(input.Item.TicketId));
            if(existing.Version != input.Item.ExpectedVersion)
            {
                throw new OptimisticConcurrencyException(
                                string.Format(
                                        "Could not update ticket '{0}' because DB version '{1}' does not match expected version '{2}'",
                                        input.Item.TicketId,
                                        existing.Version,
                                        input.Item.ExpectedVersion));
            }

            _implementation.Execute(input);
        }
    }
}
