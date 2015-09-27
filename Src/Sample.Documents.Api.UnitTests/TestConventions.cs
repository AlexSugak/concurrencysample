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

    public class DocumentsControllerCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Register<DocumentsController>(() => 
                new DocumentsController(
                    fixture.Create<Mock<IUserNameQuery>>().Object,
                    fixture.Create<Mock<IGetAllDocumentsQuery>>().Object,
                    fixture.Create<Mock<IGetDocumentQuery>>().Object,
                    fixture.Create<Mock<ICommand<Document>>>().Object,
                    fixture.Create<Mock<ICommand<Document>>>().Object,
                    fixture.Create<Mock<ICommand<DocumentReference>>>().Object
                    ));
        }
    }
}
