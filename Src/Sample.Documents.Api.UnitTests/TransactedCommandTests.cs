using Moq;
using Sample.Documents.Api.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Extensions;

namespace Sample.Documents.Api.UnitTests
{
    public class TransactedCommandTests
    {
        [Theory]
        [MoqAutoData]
        public void execute_calls_inner_command(
            Mock<ICommand<Document>> inner,
            Envelope<Document> doc)
        {
            var sut = new TransactedCommand<Document>(inner.Object);

            sut.Execute(doc);

            inner.Verify(cmd => cmd.Execute(doc), Times.Once);
        }
    }
}
