using Sample.Api.Shared;
using Sample.Tickets.Api.Commands;
using Sample.Tickets.Api.Controllers;
using Sample.Tickets.Api.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace Sample.Tickets.Api
{
    public class CompositionRoot : IHttpControllerActivator
    {
        private readonly IEnvelop _envelop;
        private readonly IUserNameQuery _userQuery;
        private readonly IQuery<EmptyRequest, IEnumerable<TicketDetails>> _allTicketsQuery;
        private readonly IQuery<Guid, TicketDetails> _getTicketQuery;
        private readonly IQuery<HttpRequestMessage, ulong> _getVersionQuery;
        private readonly ICommand<Ticket> _addTicketCmd;
        private readonly ICommand<Ticket> _updateTicketCmd;
        private readonly ICommand<TicketReference> _deleteTicketCmd;

        public CompositionRoot(
            IEnvelop envelop,
            IUserNameQuery userQuery,
            IQuery<EmptyRequest, IEnumerable<TicketDetails>> allTicketsQuery,
            IQuery<Guid, TicketDetails> getTicketQuery,
            IQuery<HttpRequestMessage, ulong> getVersionQuery,
            ICommand<Ticket> addTicketCmd,
            ICommand<Ticket> updateTicketCmd,
            ICommand<TicketReference> deleteTicketCmd)
        {
            _envelop = envelop;
            _userQuery = userQuery;
            _allTicketsQuery = allTicketsQuery;
            _getTicketQuery = getTicketQuery;
            _addTicketCmd = addTicketCmd;
            _updateTicketCmd = updateTicketCmd;
            _deleteTicketCmd = deleteTicketCmd;
            _getVersionQuery = getVersionQuery;
        }

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            if(controllerType == typeof(HomeController))
            {
                return new HomeController();
            }

            if (controllerType == typeof(TicketsController))
            {
                return new TicketsController(
                    _envelop,
                    _allTicketsQuery,
                    _getTicketQuery,
                    _getVersionQuery,
                    _addTicketCmd,
                    _updateTicketCmd,
                    _deleteTicketCmd);
            }

            throw new NotSupportedException(string.Format("Controller type {0} not supported", controllerType.FullName));
        }
    }
}
