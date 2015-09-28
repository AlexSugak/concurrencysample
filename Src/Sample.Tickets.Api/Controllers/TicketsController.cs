using AutoMapper;
using Sample.Api.Shared;
using Sample.Tickets.Api.Commands;
using Sample.Tickets.Api.Exceptions;
using Sample.Tickets.Api.Queries;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Sample.Tickets.Api.Controllers
{
    [RoutePrefix("api/tickets")]
    public class TicketsController : SecuredApiController
    {
        private readonly IGetAllTicketsQuery _allTicketsQuery;
        private readonly IGetTicketByIdQuery _getTicketQuery;
        private readonly ICommand<Ticket> _addTicketCmd;
        private readonly ICommand<Ticket> _updateTicketCmd;
        private readonly ICommand<Guid> _deleteTicketCmd;

        static TicketsController()
        {
            Mapper.CreateMap<TicketDetails, TicketResponseModel>();
            Mapper.CreateMap<TicketModel, Ticket>();
        }

        public TicketsController(
            IUserNameQuery userQuery,
            IGetAllTicketsQuery allTicketsQuery,
            IGetTicketByIdQuery getTicketQuery,
            ICommand<Ticket> addTicketCmd,
            ICommand<Ticket> updateTicketCmd,
            ICommand<Guid> deleteTicketCmd)
            : base(userQuery)
        {
            _getTicketQuery = getTicketQuery;
            _allTicketsQuery = allTicketsQuery;
            _addTicketCmd = addTicketCmd;
            _updateTicketCmd = updateTicketCmd;
            _deleteTicketCmd = deleteTicketCmd;
        }

        [Route]
        public IHttpActionResult Get()
        {
            return InvokeWhenUserExists(userName => this.Ok(new TicketsModel() 
            { 
                Tickets = _allTicketsQuery.Execute().Select(t => Mapper.Map<TicketResponseModel>(t)).ToList() 
            }));
        }

        [Route("{ticketId}")]
        public IHttpActionResult Get(Guid ticketId)
        {
            return InvokeWhenUserExists(userName => 
            {
                TicketDetails ticket;
                try
                {
                    ticket = _getTicketQuery.Execute(ticketId);
                }
                catch(TicketNotFoundException)
                {
                    return this.NotFound();
                }

                return new OkResultWithETag<TicketResponseModel>(
                        Mapper.Map<TicketResponseModel>(ticket),
                        this)
                    {
                        ETagValue = ticket.Version.ToString()
                    };
            });
        }

        [Route]
        public IHttpActionResult Post(TicketModel model)
        {
            var id = Guid.NewGuid();
            var newTicket = Mapper.Map<Ticket>(model);
            newTicket.TicketId = id;
            newTicket.Version = Guid.NewGuid();

            _addTicketCmd.Execute(new Envelope<Ticket>(
                                                    newTicket, 
                                                    "bob"));

            return this.Created("", model);
        }

        [Route("{ticketId}")]
        public IHttpActionResult Put(Guid ticketId, TicketModel model)
        {
            var newTicket = Mapper.Map<Ticket>(model);
            newTicket.TicketId = ticketId;
            newTicket.Version = Guid.NewGuid();

            _updateTicketCmd.Execute(new Envelope<Ticket>(
                                                    newTicket,
                                                    "bob"));

            return this.Created("", model);
        }

        [Route("{ticketId}")]
        public IHttpActionResult Delete(Guid ticketId)
        {
            _deleteTicketCmd.Execute(new Envelope<Guid>(
                                                    ticketId,
                                                    "bob"));

            return this.Ok();
        }
    }

    public class TicketsModel
    {
        public List<TicketResponseModel> Tickets { get; set; }
    }

    public class TicketModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string AssignedTo { get; set; }
        public string Severity { get; set; }
        public string Status { get; set; }
    }

    public class TicketResponseModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string AssignedTo { get; set; }
        public string Severity { get; set; }
        public string Status { get; set; }
    }
}
