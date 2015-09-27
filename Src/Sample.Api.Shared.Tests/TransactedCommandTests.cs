using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Extensions;

namespace Sample.Api.Shared.Tests
{
    public class TransactedCommandTests
    {
        public class TestDto
        {
            public string Something { get; set; }
        }

        [Theory]
        [MoqAutoData]
        public void execute_calls_inner_command(
            Mock<ICommand<TestDto>> inner,
            Envelope<TestDto> doc)
        {
            var sut = new TransactedCommand<TestDto>(inner.Object);

            sut.Execute(doc);

            inner.Verify(cmd => cmd.Execute(doc), Times.Once);
        }
    }
}
