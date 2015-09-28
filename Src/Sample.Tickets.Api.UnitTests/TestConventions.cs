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

namespace Sample.Tickets.Api.UnitTests
{
    public class TicketsControllerAutoDataAttribute : MoqAutoDataAttribute
    {
        public TicketsControllerAutoDataAttribute()
            : base(new Fixture()
                        .Customize(new TicketsControllerCustomization()))
        {
        }
    }

    public class TicketsControllerCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Register<TicketsController>(() =>
                new TicketsController(
                    fixture.Create<Mock<IUserNameQuery>>().Object,
                    fixture.Create<Mock<IGetAllTicketsQuery>>().Object,
                    fixture.Create<Mock<IGetTicketByIdQuery>>().Object,
                    fixture.Create<Mock<ICommand<Ticket>>>().Object,
                    fixture.Create<Mock<ICommand<Ticket>>>().Object,
                    fixture.Create<Mock<ICommand<Guid>>>().Object
                    ));
        }
    }
}
