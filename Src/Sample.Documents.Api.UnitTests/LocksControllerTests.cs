using Moq;
using Sample.Documents.Api.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;
using FluentAssertions;
using System.Web.Http.Results;
using Sample.Documents.Api.Commands;
using Sample.Documents.Api.Exceptions;
using System.Net;
using System.Net.Http;
using Sample.Documents.Api.Controllers;

namespace Sample.Documents.Api.UnitTests
{
    public class LocksControllerTests
    {
        [Theory]
        [MoqAutoData]
        public void PUT_returns_correct_result_when_no_auth_header_in_request(
            Guid documentId,
            Mock<IUserNameQuery> userQuery,
            Mock<IPutLockOnDocumentCommand> putLockCmd,
            Mock<IRemoveLockFromDocumentCommand> removeLockCmd)
        {
            var sut = new LocksController(userQuery.Object, putLockCmd.Object, removeLockCmd.Object);

            var response = sut.Put(documentId);

            response.Should().BeOfType<UnauthorizedResult>("because auth header was not specified");
        }

        [Theory]
        [MoqAutoData]
        public void PUT_returns_correct_result_when_document_already_locked(
            Guid documentId,
            string userName,
            Mock<IUserNameQuery> userQuery,
            Mock<IPutLockOnDocumentCommand> putLockCmd,
            Mock<IRemoveLockFromDocumentCommand> removeLockCmd)
        {
            putLockCmd.Setup(cmd => cmd.Execute(It.IsAny<string>(), It.IsAny<Guid>()))
                      .Throws<CannotLockAlreadyLockedDocumentException>();
            userQuery.Setup(q => q.Execute(It.IsAny<HttpRequestMessage>()))
                     .Returns(userName);

            var sut = new LocksController(userQuery.Object, putLockCmd.Object, removeLockCmd.Object);

            var response = sut.Put(documentId);

            response.Should().BeOfType<ResponseMessageResult>()
                    .Which.Response.StatusCode
                    .Should().Be(HttpStatusCode.PreconditionFailed, "because document already locked");
        }

        [Theory]
        [MoqAutoData]
        public void PUT_returns_correct_result_when_document_successfully_locked(
            Guid documentId,
            string userName,
            Mock<IUserNameQuery> userQuery,
            Mock<IPutLockOnDocumentCommand> putLockCmd,
            Mock<IRemoveLockFromDocumentCommand> removeLockCmd)
        {
            userQuery.Setup(q => q.Execute(It.IsAny<HttpRequestMessage>()))
                     .Returns(userName);

            var sut = new LocksController(userQuery.Object, putLockCmd.Object, removeLockCmd.Object);

            var response = sut.Put(documentId);

            response.Should().BeOfType<OkResult>("because locking of document succeeded");
        }

        [Theory]
        [MoqAutoData]
        public void DELETE_returns_correct_result_when_no_auth_header_in_request(
            Guid documentId,
            Mock<IUserNameQuery> userQuery,
            Mock<IPutLockOnDocumentCommand> putLockCmd,
            Mock<IRemoveLockFromDocumentCommand> removeLockCmd)
        {
            var sut = new LocksController(userQuery.Object, putLockCmd.Object, removeLockCmd.Object);

            var response = sut.Delete(documentId);

            response.Should().BeOfType<UnauthorizedResult>("because auth header was not specified");
        }

        [Theory]
        [MoqAutoData]
        public void DELETE_returns_correct_result_when_document_already_locked_by_another_user(
            Guid documentId,
            string userName,
            Mock<IUserNameQuery> userQuery,
            Mock<IPutLockOnDocumentCommand> putLockCmd,
            Mock<IRemoveLockFromDocumentCommand> removeLockCmd)
        {
            removeLockCmd.Setup(cmd => cmd.Execute(It.IsAny<string>(), It.IsAny<Guid>()))
                      .Throws<CannotRemoveAnotherUsersLockException>();
            userQuery.Setup(q => q.Execute(It.IsAny<HttpRequestMessage>()))
                     .Returns(userName);

            var sut = new LocksController(userQuery.Object, putLockCmd.Object, removeLockCmd.Object);

            var response = sut.Delete(documentId);

            response.Should().BeOfType<ResponseMessageResult>()
                    .Which.Response.StatusCode
                    .Should().Be(HttpStatusCode.PreconditionFailed, "because document locked by another user");
        }

        [Theory]
        [MoqAutoData]
        public void DELETE_returns_correct_result_when_document_lock_was_removed(
            Guid documentId,
            string userName,
            Mock<IUserNameQuery> userQuery,
            Mock<IPutLockOnDocumentCommand> putLockCmd,
            Mock<IRemoveLockFromDocumentCommand> removeLockCmd)
        {
            userQuery.Setup(q => q.Execute(It.IsAny<HttpRequestMessage>()))
                     .Returns(userName);

            var sut = new LocksController(userQuery.Object, putLockCmd.Object, removeLockCmd.Object);

            var response = sut.Delete(documentId);

            response.Should().BeOfType<ResponseMessageResult>()
                    .Which.Response.StatusCode
                    .Should().Be(HttpStatusCode.NoContent, "because document locked was removed");
        }
    }
}
