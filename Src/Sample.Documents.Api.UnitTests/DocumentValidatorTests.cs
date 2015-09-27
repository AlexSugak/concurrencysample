using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using Sample.Api.Shared;
using Sample.Api.Shared.Tests;
using Sample.Documents.Api.Commands;
using Sample.Documents.Api.Exceptions;
using Xunit.Extensions;

namespace Sample.Documents.Api.UnitTests
{
    public class DocumentValidatorTests
    {
        [Theory]
        [MoqAutoData]
        public void execute_throws_exception_on_null_title(
            Envelope<Document> doc,
            Mock<ICommand<Document>> impl)
        {
            doc.Item.Title = null;
            var sut = new DocumentValidator(impl.Object);

            sut.Invoking(s => s.Execute(doc)).ShouldThrow<ValidationException>("because title is required");
        }

        [Theory]
        [MoqAutoData]
        public void execute_throws_exception_on_empty_title(
            Envelope<Document> doc,
            Mock<ICommand<Document>> impl)
        {
            doc.Item.Title = "";
            var sut = new DocumentValidator(impl.Object);

            sut.Invoking(s => s.Execute(doc)).ShouldThrow<ValidationException>("because title is required");
        }

        [Theory]
        [MoqAutoData]
        public void execute_throws_exception_on_null_content(
            Envelope<Document> doc,
            Mock<ICommand<Document>> impl)
        {
            doc.Item.Content = null;
            var sut = new DocumentValidator(impl.Object);

            sut.Invoking(s => s.Execute(doc)).ShouldThrow<ValidationException>("because content is required");
        }

        [Theory]
        [MoqAutoData]
        public void execute_throws_exception_on_empty_content(
            Envelope<Document> doc,
            Mock<ICommand<Document>> impl)
        {
            doc.Item.Content = "";
            var sut = new DocumentValidator(impl.Object);

            sut.Invoking(s => s.Execute(doc)).ShouldThrow<ValidationException>("because content is required");
        }

        [Theory]
        [MoqAutoData]
        public void execute_throws_exception_on_empty_id(
            Envelope<Document> doc,
            Mock<ICommand<Document>> impl)
        {
            doc.Item.DocumentId = Guid.Empty;
            var sut = new DocumentValidator(impl.Object);

            sut.Invoking(s => s.Execute(doc)).ShouldThrow<ValidationException>("because id is required");
        }

        [Theory]
        [MoqAutoData]
        public void execute_calls_inner_command_when_doc_is_valid(
            Envelope<Document> doc,
            Mock<ICommand<Document>> impl)
        {
            var sut = new DocumentValidator(impl.Object);
            sut.Execute(doc);

            impl.Verify(cmd => cmd.Execute(doc), Times.Once);
        }
    }
}
