using Sample.Documents.Api.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Results;
using FluentAssertions;
using Moq;
using Sample.Api.Shared;
using Sample.Api.Shared.Tests;
using Sample.Documents.Api.Commands;
using Sample.Documents.Api.Exceptions;
using Sample.Documents.Api.Controllers;
using Xunit;
using Xunit.Extensions;

namespace Sample.Documents.Api.UnitTests
{
    public class LocksControllerTests
    {
        [Theory]
        [MoqAutoData]
        public void PUT_returns_correct_result_when_no_auth_header_in_request(
            Guid documentId,
            Mock<IUserNameQuery> userQuery,
            Mock<ICommand<LockInfo>> putLockCmd,
            Mock<ICommand<LockInfo>> removeLockCmd)
        {
            var sut = new LocksController(userQuery.Object, putLockCmd.Object, removeLockCmd.Object);

            var response = sut.Put(documentId);

            response.Should().BeOfType<UnauthorizedResult>("because auth header was not specified");
        }

        [Theory]
        [MoqAutoData]
        public void PUT_returns_correct_result_on_lock_exception(
            Guid documentId,
            string userName,
            Mock<IUserNameQuery> userQuery,
            Mock<ICommand<LockInfo>> putLockCmd,
            Mock<ICommand<LockInfo>> removeLockCmd)
        {
            putLockCmd.Setup(cmd => cmd.Execute(It.IsAny<Envelope<LockInfo>>()))
                      .Throws<DocumentLockedException>();
            userQuery.Setup(q => q.Execute(It.IsAny<HttpRequestMessage>()))
                     .Returns(userName);

            var sut = new LocksController(userQuery.Object, putLockCmd.Object, removeLockCmd.Object);

            var response = sut.Put(documentId);

            response.Should().BeOfType<ResponseMessageResult>()
                    .Which.Response.StatusCode
                    .Should().Be(HttpStatusCode.PreconditionFailed, "because document already locked");

            response.Should().BeOfType<ResponseMessageResult>()
                    .Which.Response.Content.ReadAsStringAsync().Result
                    .Should().NotBeNullOrEmpty("becase error message should be provided");
        }

        [Theory]
        [MoqAutoData]
        public void PUT_returns_correct_result_when_document_successfully_locked(
            Guid documentId,
            string userName,
            Mock<IUserNameQuery> userQuery,
            Mock<ICommand<LockInfo>> putLockCmd,
            Mock<ICommand<LockInfo>> removeLockCmd)
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
            Mock<ICommand<LockInfo>> putLockCmd,
            Mock<ICommand<LockInfo>> removeLockCmd)
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
            Mock<ICommand<LockInfo>> putLockCmd,
            Mock<ICommand<LockInfo>> removeLockCmd)
        {
            removeLockCmd.Setup(cmd => cmd.Execute(It.IsAny<Envelope<LockInfo>>()))
                      .Throws<DocumentLockedException>();
            userQuery.Setup(q => q.Execute(It.IsAny<HttpRequestMessage>()))
                     .Returns(userName);

            var sut = new LocksController(userQuery.Object, putLockCmd.Object, removeLockCmd.Object);

            var response = sut.Delete(documentId);

            response.Should().BeOfType<ResponseMessageResult>()
                    .Which.Response.StatusCode
                    .Should().Be(HttpStatusCode.PreconditionFailed, "because document locked by another user");

            response.Should().BeOfType<ResponseMessageResult>()
                    .Which.Response.Content.ReadAsStringAsync().Result
                    .Should().NotBeNullOrEmpty("becase error message should be provided");
        }

        [Theory]
        [MoqAutoData]
        public void DELETE_returns_correct_result_when_document_lock_was_removed(
            Guid documentId,
            string userName,
            Mock<IUserNameQuery> userQuery,
            Mock<ICommand<LockInfo>> putLockCmd,
            Mock<ICommand<LockInfo>> removeLockCmd)
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
