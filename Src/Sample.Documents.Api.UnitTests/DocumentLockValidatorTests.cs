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
    public class DocumentLockValidatorTests
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
            var lockInfo = new Envelope<Lock>(new Lock(documentId), userName);
            document.CheckedOutBy = null;
            docQuery.Setup(q => q.Execute(documentId)).Returns(document);

            var sut = new DocumentLockValidator<Lock>(inner.Object, docQuery.Object);

            sut.Execute(lockInfo);

            inner.Verify(cmd => cmd.Execute(lockInfo), Times.Once);
        }

        [Theory]
        [MoqAutoData]
        public void execute_calls_innder_implementation_when_document_is_already_locked_by_the_same_user(
            string userName,
            Guid documentId,
            DocumentDetails document,
            Mock<IGetDocumentQuery> docQuery,
            Mock<ICommand<Lock>> inner)
        {
            var lockInfo = new Envelope<Lock>(new Lock(documentId), userName);
            document.CheckedOutBy = userName;
            docQuery.Setup(q => q.Execute(documentId)).Returns(document);

            var sut = new DocumentLockValidator<Lock>(inner.Object, docQuery.Object);

            sut.Execute(lockInfo);

            inner.Verify(cmd => cmd.Execute(lockInfo), Times.Once);
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
            var lockInfo = new Envelope<Lock>(new Lock(documentId), userName);
            document.CheckedOutBy = anotherUserName;
            docQuery.Setup(q => q.Execute(documentId)).Returns(document);

            var sut = new DocumentLockValidator<Lock>(inner.Object, docQuery.Object);

            sut.Invoking(cmd => cmd.Execute(lockInfo))
               .ShouldThrow<DocumentLockedException>();
        }
    }
}
