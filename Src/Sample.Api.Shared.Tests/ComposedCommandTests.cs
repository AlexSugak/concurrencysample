using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Sample.Api.Shared;
using Xunit;
using Xunit.Extensions;

namespace Sample.Api.Shared.Tests
{
    public class ComposedCommandTests
    {
        public class TestDto
        {
            public string Something { get; set; }
        }

        [Theory]
        [MoqAutoData]
        public void execute_calls_all_inner_commands(
            Envelope<TestDto> doc,
            List<Mock<ICommand<TestDto>>> commands)
        {
            var sut = new ComposedCommand<TestDto>(commands.Select(cmd => cmd.Object).ToArray());
            sut.Execute(doc);

            Assert.True(commands.Count > 1);
            foreach(var cmd in commands)
            {
                cmd.Verify(c => c.Execute(doc), Times.Once);
            }
        }
    }
}
