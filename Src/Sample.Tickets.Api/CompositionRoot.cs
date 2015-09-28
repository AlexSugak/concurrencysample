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
        private readonly IUserNameQuery _userQuery;
        private readonly IGetAllTicketsQuery _allTicketsQuery;
        private readonly IGetTicketByIdQuery _getTicketQuery;
        private readonly ICommand<Ticket> _addTicketCmd;
        private readonly ICommand<Ticket> _updateTicketCmd;
        private readonly ICommand<Guid> _deleteTicketCmd;

        public CompositionRoot(
            IUserNameQuery userQuery,
            IGetAllTicketsQuery allTicketsQuery,
            IGetTicketByIdQuery getTicketQuery,
            ICommand<Ticket> addTicketCmd,
            ICommand<Ticket> updateTicketCmd,
            ICommand<Guid> deleteTicketCmd)
        {
            _userQuery = userQuery;
            _allTicketsQuery = allTicketsQuery;
            _getTicketQuery = getTicketQuery;
            _addTicketCmd = addTicketCmd;
            _updateTicketCmd = updateTicketCmd;
            _deleteTicketCmd = deleteTicketCmd;
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
                    _userQuery, 
                    _allTicketsQuery, 
                    _getTicketQuery, 
                    _addTicketCmd, 
                    _updateTicketCmd,
                    _deleteTicketCmd);
            }

            throw new NotSupportedException(string.Format("Controller type {0} not supported", controllerType.FullName));
        }
    }
}
