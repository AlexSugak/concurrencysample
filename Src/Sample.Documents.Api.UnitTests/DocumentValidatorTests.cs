using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Sample.Documents.Api.Commands;
using Xunit.Extensions;

namespace Sample.Documents.Api.UnitTests
{
    public class DocumentValidatorTests
    {
        [Theory]
        [MoqAutoData]
        public void execute_throws_exception_on_null_title(
            Document doc,
            Mock<ICommand<Document>> impl)
        {
            doc.Title = null;
            var sut = new DocumentValidator(impl.Object);

            sut.Invoking(s => s.Execute(doc)).ShouldThrow<ValidationException>("because title is required");
        }

        [Theory]
        [MoqAutoData]
        public void execute_throws_exception_on_empty_title(
            Document doc,
            Mock<ICommand<Document>> impl)
        {
            doc.Title = "";
            var sut = new DocumentValidator(impl.Object);

            sut.Invoking(s => s.Execute(doc)).ShouldThrow<ValidationException>("because title is required");
        }

        [Theory]
        [MoqAutoData]
        public void execute_throws_exception_on_null_content(
            Document doc,
            Mock<ICommand<Document>> impl)
        {
            doc.Content = null;
            var sut = new DocumentValidator(impl.Object);

            sut.Invoking(s => s.Execute(doc)).ShouldThrow<ValidationException>("because content is required");
        }

        [Theory]
        [MoqAutoData]
        public void execute_throws_exception_on_empty_content(
            Document doc,
            Mock<ICommand<Document>> impl)
        {
            doc.Content = "";
            var sut = new DocumentValidator(impl.Object);

            sut.Invoking(s => s.Execute(doc)).ShouldThrow<ValidationException>("because content is required");
        }

        [Theory]
        [MoqAutoData]
        public void execute_throws_exception_on_empty_id(
            Document doc,
            Mock<ICommand<Document>> impl)
        {
            doc.Id = Guid.Empty;
            var sut = new DocumentValidator(impl.Object);

            sut.Invoking(s => s.Execute(doc)).ShouldThrow<ValidationException>("because id is required");
        }

        [Theory]
        [MoqAutoData]
        public void execute_calls_inner_command_when_doc_is_valid(
            Document doc,
            Mock<ICommand<Document>> impl)
        {
            var sut = new DocumentValidator(impl.Object);
            sut.Execute(doc);

            impl.Verify(cmd => cmd.Execute(doc), Times.Once);
        }
    }
}
