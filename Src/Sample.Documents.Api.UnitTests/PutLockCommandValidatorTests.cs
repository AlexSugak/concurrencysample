using Moq;
using Sample.Documents.Api.Commands;
using Sample.Documents.Api.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Extensions;
using FluentAssertions;
using Sample.Documents.Api.Exceptions;

namespace Sample.Documents.Api.UnitTests
{
    public class PutLockCommandValidatorTests
    {
        [Theory]
        [MoqAutoData]
        public void execute_calls_inner_implementation_when_document_is_not_locked(
            string userName,
            Guid documentId,
            DocumentDetails document,
            Mock<IGetDocumentQuery> docQuery,
            Mock<ICommand<Lock>> inner)
        {
            document.CheckedOutBy = null;
            docQuery.Setup(q => q.Execute(documentId)).Returns(document);

            var sut = new PutLockCommandValidator(inner.Object, docQuery.Object);

            var l = new Lock(userName, documentId);
            sut.Execute(l);

            inner.Verify(cmd => cmd.Execute(l), Times.Once);
        }

        [Theory]
        [MoqAutoData]
        public void execute_does_nothing_when_document_is_already_locked_by_the_same_user(
            string userName,
            Guid documentId,
            DocumentDetails document,
            Mock<IGetDocumentQuery> docQuery,
            Mock<ICommand<Lock>> inner)
        {
            document.CheckedOutBy = userName;
            docQuery.Setup(q => q.Execute(documentId)).Returns(document);

            var sut = new PutLockCommandValidator(inner.Object, docQuery.Object);

            var l = new Lock(userName, documentId);
            sut.Execute(l);

            inner.Verify(cmd => cmd.Execute(l), Times.Never);
        }

        [Theory]
        [MoqAutoData]
        public void execute_throws_exception_when_document_is_locked_by_another_user(
            string userName,
            string anotherUserName,
            Guid documentId,
            DocumentDetails document,
            Mock<IGetDocumentQuery> docQuery,
            Mock<ICommand<Lock>> inner)
        {
            document.CheckedOutBy = anotherUserName;
            docQuery.Setup(q => q.Execute(documentId)).Returns(document);

            var sut = new PutLockCommandValidator(inner.Object, docQuery.Object);

            sut.Invoking(cmd => cmd.Execute(new Lock(userName, documentId)))
               .ShouldThrow<CannotLockAlreadyLockedDocumentException>();
        }
    }
}
