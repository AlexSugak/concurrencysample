using AutoMapper;
using Sample.Api.Shared;
using Sample.Tickets.Api.Commands;
using Sample.Tickets.Api.Exceptions;
using Sample.Tickets.Api.Queries;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
        private readonly ICommand<TicketReference> _deleteTicketCmd;
        private readonly IGetTicketVersionQuery _versionQuery;

        static TicketsController()
        {
            Mapper.CreateMap<TicketDetails, TicketResponseModel>()
                        .ForMember(r => r.ETag, d => d.MapFrom(t => "\"" + t.Version + "\""));
            Mapper.CreateMap<TicketModel, Ticket>();
        }

        public TicketsController(
            IUserNameQuery userQuery,
            IGetAllTicketsQuery allTicketsQuery,
            IGetTicketByIdQuery getTicketQuery,
            ICommand<Ticket> addTicketCmd,
            ICommand<Ticket> updateTicketCmd,
            ICommand<TicketReference> deleteTicketCmd,
            IGetTicketVersionQuery versionQuery)
            : base(userQuery)
        {
            _getTicketQuery = getTicketQuery;
            _allTicketsQuery = allTicketsQuery;
            _addTicketCmd = addTicketCmd;
            _updateTicketCmd = updateTicketCmd;
            _deleteTicketCmd = deleteTicketCmd;
            _versionQuery = versionQuery;
        }

        [Route]
        public IHttpActionResult Get()
        {
            return InvokeWhenUserExists(userName => this.Ok(new TicketsModel() 
            { 
                Tickets = _allTicketsQuery.Execute().Select(t => Mapper.Map<TicketResponseModel>(t)).ToList() 
            }));
        }

        [Route("{ticketId}", Name="GetTicketById")]
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

                var ticketResponse = Mapper.Map<TicketResponseModel>(ticket);
                return new OkResultWithETag<TicketResponseModel>(ticketResponse, this)
                {
                    ETagValue = ticket.Version.ToString()
                };
            });
        }

        [Route]
        public IHttpActionResult Post(TicketModel model)
        {
            return InvokeWhenUserExists(userName =>
            {
                var id = Guid.NewGuid();
                var newTicket = Mapper.Map<Ticket>(model);
                newTicket.TicketId = id;

                try
                {
                    _addTicketCmd.Execute(new Envelope<Ticket>(newTicket, userName));
                }
                catch(ValidationException e)
                {
                    return this.BadRequest(e.Message);
                }

                var ticket = _getTicketQuery.Execute(id);
                var response = Mapper.Map<TicketResponseModel>(ticket);

                var uri = this.Url.Link("GetTicketById", new { ticketId = id });
                return new CreatedResultWithETag<TicketResponseModel>(new Uri(uri), response, this)
                {
                    ETagValue = ticket.Version.ToString()
                };
            });
        }

        [Route("{ticketId}")]
        public IHttpActionResult Put(Guid ticketId, TicketModel model)
        {
            return InvokeWhenUserExists(userName =>
            {
                var newTicket = Mapper.Map<Ticket>(model);
                newTicket.TicketId = ticketId;
                newTicket.ExpectedVersion = _versionQuery.Execute(this.Request);

                try
                {
                    _updateTicketCmd.Execute(new Envelope<Ticket>(newTicket, userName));
                }
                catch(ValidationException e)
                {
                    return this.BadRequest(e.Message);
                }
                catch(TicketNotFoundException)
                {
                    return this.NotFound();
                }
                catch(OptimisticConcurrencyException e)
                {
                    return this.ResponseMessage(new HttpResponseMessage(HttpStatusCode.PreconditionFailed) 
                    { 
                        Content = new StringContent(e.Message) 
                    });
                }

                var ticket = _getTicketQuery.Execute(ticketId);
                var response = Mapper.Map<TicketResponseModel>(ticket);

                return new OkResultWithETag<TicketResponseModel>(response, this)
                {
                    ETagValue = ticket.Version.ToString()
                };
            });
        }

        [Route("{ticketId}")]
        public IHttpActionResult Delete(Guid ticketId)
        {
            return InvokeWhenUserExists(userName =>
            {
                try
                {
                    _deleteTicketCmd.Execute(new Envelope<TicketReference>(
                                                            new TicketReference(
                                                                ticketId, 
                                                                _versionQuery.Execute(this.Request)), 
                                                            userName));
                }
                catch(OptimisticConcurrencyException e)
                {
                    return this.ResponseMessage(new HttpResponseMessage(HttpStatusCode.PreconditionFailed)
                    {
                        Content = new StringContent(e.Message)
                    });
                }

                return this.ResponseMessage(new HttpResponseMessage(HttpStatusCode.NoContent));
            });
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
        public string ETag { get; set; }
    }
}
