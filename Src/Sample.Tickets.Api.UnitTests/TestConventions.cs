using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Xunit;
using Sample.Api.Shared;
using Sample.Api.Shared.Tests;
using Sample.Tickets.Api.Controllers;
using Sample.Tickets.Api.Queries;
using Sample.Tickets.Api.Commands;
using System.Web.Http.Routing;
using System.Net.Http;

namespace Sample.Tickets.Api.UnitTests
{
    public class TicketsControllerAutoDataAttribute : MoqAutoDataAttribute
    {
        public TicketsControllerAutoDataAttribute()
            : base(new Fixture().Customize(new TicketsControllerCustomization()))
        {
        }
    }

    public class TestEnvelop : IEnvelop
    {
        public Envelope<T> Envelop<T>(HttpRequestMessage request, T item)
        {
            return new Envelope<T>(item, "foo");
        }
    }

    public class TicketsControllerCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Register<TicketsController>(() =>
                new TicketsController(
                    new TestEnvelop(),
                    fixture.Create<Mock<IQuery<EmptyRequest, IEnumerable<TicketDetails>>>>().Object,
                    fixture.Create<Mock<IQuery<Guid, TicketDetails>>>().Object,
                    fixture.Create<Mock<IQuery<HttpRequestMessage, ulong>>>().Object,
                    fixture.Create<Mock<ICommand<Ticket>>>().Object,
                    fixture.Create<Mock<ICommand<Ticket>>>().Object,
                    fixture.Create<Mock<ICommand<TicketReference>>>().Object
                    ) { Url = fixture.Create<Mock<UrlHelper>>().Object });
        }
    }
}
