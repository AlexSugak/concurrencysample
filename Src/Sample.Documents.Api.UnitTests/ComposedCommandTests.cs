using Moq;
using Sample.Documents.Api.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;

namespace Sample.Documents.Api.UnitTests
{
    public class ComposedCommandTests
    {
        [Theory]
        [MoqAutoData]
        public void execute_calls_all_inner_commands(
            Envelope<Document> doc,
            List<Mock<ICommand<Document>>> commands)
        {
            var sut = new ComposedCommand<Document>(commands.Select(cmd => cmd.Object).ToArray());
            sut.Execute(doc);

            Assert.True(commands.Count > 1);
            foreach(var cmd in commands)
            {
                cmd.Verify(c => c.Execute(doc), Times.Once);
            }
        }
    }
}
