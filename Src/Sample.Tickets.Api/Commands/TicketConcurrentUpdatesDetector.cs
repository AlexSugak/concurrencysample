﻿using Sample.Api.Shared;
using Sample.Tickets.Api.Exceptions;
using Sample.Tickets.Api.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Tickets.Api.Commands
{
    public class TicketConcurrentUpdatesDetector<T> : ICommand<T> where T : ITicketReference
    {
        private readonly ICommand<T> _implementation;
        private readonly IGetTicketByIdQuery _getTicketQuery;

        public TicketConcurrentUpdatesDetector(
            ICommand<T> implementation,
            IGetTicketByIdQuery getTicketQuery)
        {
            _implementation = implementation;
            _getTicketQuery = getTicketQuery;
        }

        public void Execute(Envelope<T> input)
        {
            var existing = _getTicketQuery.Execute(input.Item.TicketId);

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