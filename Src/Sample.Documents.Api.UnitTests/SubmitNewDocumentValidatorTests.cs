﻿using System;
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
    public class SubmitNewDocumentValidatorTests
    {
        [Theory]
        [MoqAutoData]
        public void execute_throws_exception_on_null_title(
            NewDocument doc,
            Mock<ISubmitNewDocumentCommand> impl)
        {
            doc.Title = null;
            var sut = new SubmitNewDocumentValidator(impl.Object);

            sut.Invoking(s => s.Execute(doc)).ShouldThrow<ValidationException>("because title is required");
        }

        [Theory]
        [MoqAutoData]
        public void execute_throws_exception_on_empty_title(
            NewDocument doc,
            Mock<ISubmitNewDocumentCommand> impl)
        {
            doc.Title = "";
            var sut = new SubmitNewDocumentValidator(impl.Object);

            sut.Invoking(s => s.Execute(doc)).ShouldThrow<ValidationException>("because title is required");
        }

        [Theory]
        [MoqAutoData]
        public void execute_throws_exception_on_null_content(
            NewDocument doc,
            Mock<ISubmitNewDocumentCommand> impl)
        {
            doc.Content = null;
            var sut = new SubmitNewDocumentValidator(impl.Object);

            sut.Invoking(s => s.Execute(doc)).ShouldThrow<ValidationException>("because content is required");
        }

        [Theory]
        [MoqAutoData]
        public void execute_throws_exception_on_empty_content(
            NewDocument doc,
            Mock<ISubmitNewDocumentCommand> impl)
        {
            doc.Content = "";
            var sut = new SubmitNewDocumentValidator(impl.Object);

            sut.Invoking(s => s.Execute(doc)).ShouldThrow<ValidationException>("because content is required");
        }

        [Theory]
        [MoqAutoData]
        public void execute_throws_exception_on_empty_id(
            NewDocument doc,
            Mock<ISubmitNewDocumentCommand> impl)
        {
            doc.Id = Guid.Empty;
            var sut = new SubmitNewDocumentValidator(impl.Object);

            sut.Invoking(s => s.Execute(doc)).ShouldThrow<ValidationException>("because id is required");
        }

        [Theory]
        [MoqAutoData]
        public void execute_calls_inner_command_when_doc_is_valid(
            NewDocument doc,
            Mock<ISubmitNewDocumentCommand> impl)
        {
            var sut = new SubmitNewDocumentValidator(impl.Object);
            sut.Execute(doc);

            impl.Verify(cmd => cmd.Execute(doc), Times.Once);
        }
    }
}
