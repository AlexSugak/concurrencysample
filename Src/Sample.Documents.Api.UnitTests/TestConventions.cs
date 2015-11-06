using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Xunit;
using Sample.Api.Shared;
using Sample.Api.Shared.Tests;
using Sample.Documents.Api.Commands;
using Sample.Documents.Api.Controllers;
using Sample.Documents.Api.Queries;

namespace Sample.Documents.Api.UnitTests
{
    public class DocumentsControllerAutoDataAttribute : MoqAutoDataAttribute
    {
        public DocumentsControllerAutoDataAttribute()
            : base(new Fixture()
                        .Customize(new DocumentsControllerCustomization()))
        {
        }
    }

    public class LocksControllerAutoDataAttribute : MoqAutoDataAttribute
    {
        public LocksControllerAutoDataAttribute()
            : base(new Fixture()
                        .Customize(new LocksControllerCustomization()))
        {
        }
    }

    public class DocumentsControllerCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Register<DocumentsController>(() => 
                new DocumentsController(
                    new TestEnvelop(),
                    fixture.Create<Mock<IQuery<EmptyRequest, IEnumerable<DocumentDetails>>>>().Object,
                    fixture.Create<Mock<IQuery<Guid, DocumentDetails>>>().Object,
                    fixture.Create<Mock<ICommand<Document>>>().Object,
                    fixture.Create<Mock<ICommand<Document>>>().Object,
                    fixture.Create<Mock<ICommand<DocumentReference>>>().Object
                    ));
        }
    }

    public class LocksControllerCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Register<LocksController>(() =>
                new LocksController(
                    new TestEnvelop(),
                    fixture.Create<Mock<ICommand<LockInfo>>>().Object,
                    fixture.Create<Mock<ICommand<LockInfo>>>().Object
                    ));
        }
    }

    public class TestEnvelop : IEnvelop
    {
        public Envelope<T> Envelop<T>(HttpRequestMessage request, T item)
        {
            return new Envelope<T>(item, "foo");
        }
    }
}
