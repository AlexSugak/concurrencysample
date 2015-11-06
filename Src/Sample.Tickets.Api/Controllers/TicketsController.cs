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
    public class TicketsController : ApiControllerWithEnvelope
    {
        private readonly IQuery<EmptyRequest, IEnumerable<TicketDetails>> _allTicketsQuery;
        private readonly IQuery<Guid, TicketDetails> _getTicketQuery;
        private readonly IQuery<HttpRequestMessage, ulong> _versionQuery;
        private readonly ICommand<Ticket> _addTicketCmd;
        private readonly ICommand<Ticket> _updateTicketCmd;
        private readonly ICommand<TicketReference> _deleteTicketCmd;

        static TicketsController()
        {
            Mapper.CreateMap<TicketDetails, TicketResponseModel>()
                        .ForMember(r => r.ETag, d => d.MapFrom(t => "\"" + t.Version + "\""));
            Mapper.CreateMap<TicketModel, Ticket>();
        }

        public TicketsController(
            IEnvelop envelop,
            IQuery<EmptyRequest, IEnumerable<TicketDetails>> allTicketsQuery,
            IQuery<Guid, TicketDetails> getTicketQuery,
            IQuery<HttpRequestMessage, ulong> versionQuery,
            ICommand<Ticket> addTicketCmd,
            ICommand<Ticket> updateTicketCmd,
            ICommand<TicketReference> deleteTicketCmd)
            : base(envelop)
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
            IEnumerable<TicketDetails> tickets;
            try
            {
                tickets = _allTicketsQuery.Execute(Envelop(new EmptyRequest()));
            }
            catch (UnauthorizedAccessException)
            {
                return this.Unauthorized();
            }

            return this.Ok(new TicketsModel() 
            { 
                Tickets = tickets.Select(t => Mapper.Map<TicketResponseModel>(t)).ToList() 
            });
        }

        [Route("{ticketId}", Name = "GetTicketById")]
        public IHttpActionResult Get(Guid ticketId)
        {
            TicketDetails ticket;
            try
            {
                ticket = _getTicketQuery.Execute(Envelop(ticketId));
            }
            catch (UnauthorizedAccessException)
            {
                return this.Unauthorized();
            }
            catch (TicketNotFoundException)
            {
                return this.NotFound();
            }

            var ticketResponse = Mapper.Map<TicketResponseModel>(ticket);
            return new OkResultWithETag<TicketResponseModel>(ticketResponse, this)
            {
                ETagValue = ticket.Version.ToString()
            };
        }

        [Route]
        public IHttpActionResult Post(TicketModel model)
        {
            var id = Guid.NewGuid();
            var newTicket = Mapper.Map<Ticket>(model);
            newTicket.TicketId = id;

            try
            {
                _addTicketCmd.Execute(Envelop(newTicket));
            }
            catch (ValidationException e)
            {
                return this.BadRequest(e.Message);
            }
            catch(UnauthorizedAccessException)
            {
                return this.Unauthorized();
            }

            var ticket = _getTicketQuery.Execute(Envelop(id));
            var response = Mapper.Map<TicketResponseModel>(ticket);

            var uri = this.Url.Link("GetTicketById", new { ticketId = id });
            return new CreatedResultWithETag<TicketResponseModel>(new Uri(uri), response, this)
            {
                ETagValue = ticket.Version.ToString()
            };
        }

        [Route("{ticketId}")]
        public IHttpActionResult Put(Guid ticketId, TicketModel model)
        {
            var newTicket = Mapper.Map<Ticket>(model);
            newTicket.TicketId = ticketId;
            newTicket.ExpectedVersion = _versionQuery.Execute(Envelop(this.Request));

            try
            {
                _updateTicketCmd.Execute(Envelop(newTicket));
            }
            catch (UnauthorizedAccessException)
            {
                return this.Unauthorized();
            }
            catch (ValidationException e)
            {
                return this.BadRequest(e.Message);
            }
            catch (TicketNotFoundException)
            {
                return this.NotFound();
            }
            catch (OptimisticConcurrencyException e)
            {
                return this.ResponseMessage(new HttpResponseMessage(HttpStatusCode.PreconditionFailed)
                {
                    Content = new StringContent(e.Message)
                });
            }

            var ticket = _getTicketQuery.Execute(Envelop(ticketId));
            var response = Mapper.Map<TicketResponseModel>(ticket);

            return new OkResultWithETag<TicketResponseModel>(response, this)
            {
                ETagValue = ticket.Version.ToString()
            };
        }

        [Route("{ticketId}")]
        public IHttpActionResult Delete(Guid ticketId)
        {
            try
            {
                _deleteTicketCmd.Execute(
                    Envelop(new TicketReference(ticketId, _versionQuery.Execute(Envelop(this.Request)))));
            }
            catch (UnauthorizedAccessException)
            {
                return this.Unauthorized();
            }
            catch (OptimisticConcurrencyException e)
            {
                return this.ResponseMessage(new HttpResponseMessage(HttpStatusCode.PreconditionFailed)
                {
                    Content = new StringContent(e.Message)
                });
            }

            return this.ResponseMessage(new HttpResponseMessage(HttpStatusCode.NoContent));
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
