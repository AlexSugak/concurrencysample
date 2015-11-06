using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Sample.Api.Shared;
using Sample.Api.Shared.Tests;
using Sample.Documents.Api.Commands;
using Sample.Documents.Api.Queries;
using Sample.Documents.Api.Exceptions;
using FluentAssertions;
using Xunit.Extensions;

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
            Mock<IQuery<Guid, DocumentDetails>> docQuery,
            Mock<ICommand<LockInfo>> inner)
        {
            var lockInfo = new Envelope<LockInfo>(new LockInfo(documentId), userName);
            document.CheckedOutBy = null;
            docQuery.Setup(q => q.Execute(It.Is<Envelope<Guid>>(r => r.Item == documentId))).Returns(document);

            var sut = new DocumentLockValidator<LockInfo>(inner.Object, docQuery.Object);

            sut.Execute(lockInfo);

            inner.Verify(cmd => cmd.Execute(lockInfo), Times.Once);
        }

        [Theory]
        [MoqAutoData]
        public void execute_calls_innder_implementation_when_document_is_already_locked_by_the_same_user(
            string userName,
            Guid documentId,
            DocumentDetails document,
            Mock<IQuery<Guid, DocumentDetails>> docQuery,
            Mock<ICommand<LockInfo>> inner)
        {
            var lockInfo = new Envelope<LockInfo>(new LockInfo(documentId), userName);
            document.CheckedOutBy = userName;
            docQuery.Setup(q => q.Execute(It.Is<Envelope<Guid>>(r => r.Item == documentId))).Returns(document);

            var sut = new DocumentLockValidator<LockInfo>(inner.Object, docQuery.Object);

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
            Mock<IQuery<Guid, DocumentDetails>> docQuery,
            Mock<ICommand<LockInfo>> inner)
        {
            var lockInfo = new Envelope<LockInfo>(new LockInfo(documentId), userName);
            document.CheckedOutBy = anotherUserName;
            docQuery.Setup(q => q.Execute(It.Is<Envelope<Guid>>(r => r.Item == documentId))).Returns(document);

            var sut = new DocumentLockValidator<LockInfo>(inner.Object, docQuery.Object);

            sut.Invoking(cmd => cmd.Execute(lockInfo))
               .ShouldThrow<DocumentLockedException>();
        }
    }
}
