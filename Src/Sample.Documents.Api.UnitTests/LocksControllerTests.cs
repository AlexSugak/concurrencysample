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
using Ploeh.AutoFixture.Xunit;

namespace Sample.Documents.Api.UnitTests
{
    public class LocksControllerTests
    {
        [Theory]
        [LocksControllerAutoData]
        public void PUT_returns_correct_result_when_unauthorized(
            Guid documentId,
            [Frozen]Mock<ICommand<LockInfo>> cmd,
            LocksController sut)
        {
            cmd.Setup(c => c.Execute(It.IsAny<Envelope<LockInfo>>())).Throws<UnauthorizedAccessException>();

            var response = sut.Put(documentId);

            response.Should().BeOfType<UnauthorizedResult>("because auth header was not specified");
        }

        [Theory]
        [LocksControllerAutoData]
        public void PUT_returns_correct_result_on_lock_exception(
            Guid documentId,
            [Frozen]Mock<ICommand<LockInfo>> putLockCmd,
            LocksController sut)
        {
            putLockCmd.Setup(cmd => cmd.Execute(It.IsAny<Envelope<LockInfo>>()))
                      .Throws<DocumentLockedException>();

            var response = sut.Put(documentId);

            response.Should().BeOfType<ResponseMessageResult>()
                    .Which.Response.StatusCode
                    .Should().Be(HttpStatusCode.PreconditionFailed, "because document already locked");

            response.Should().BeOfType<ResponseMessageResult>()
                    .Which.Response.Content.ReadAsStringAsync().Result
                    .Should().NotBeNullOrEmpty("becase error message should be provided");
        }

        [Theory]
        [LocksControllerAutoData]
        public void PUT_returns_correct_result_when_document_successfully_locked(
            Guid documentId,
            LocksController sut)
        {
            var response = sut.Put(documentId);

            response.Should().BeOfType<OkResult>("because locking of document succeeded");
        }

        [Theory]
        [LocksControllerAutoData]
        public void DELETE_returns_correct_result_when_unauthorized(
            Guid documentId,
            [Frozen]Mock<ICommand<LockInfo>> cmd,
            LocksController sut)
        {
            cmd.Setup(c => c.Execute(It.IsAny<Envelope<LockInfo>>())).Throws<UnauthorizedAccessException>();

            var response = sut.Delete(documentId);

            response.Should().BeOfType<UnauthorizedResult>("because auth header was not specified");
        }

        [Theory]
        [LocksControllerAutoData]
        public void DELETE_returns_correct_result_when_document_already_locked_by_another_user(
            Guid documentId,
            [Frozen]Mock<ICommand<LockInfo>> removeLockCmd,
            LocksController sut)
        {
            removeLockCmd.Setup(cmd => cmd.Execute(It.IsAny<Envelope<LockInfo>>()))
                      .Throws<DocumentLockedException>();

            var response = sut.Delete(documentId);

            response.Should().BeOfType<ResponseMessageResult>()
                    .Which.Response.StatusCode
                    .Should().Be(HttpStatusCode.PreconditionFailed, "because document locked by another user");

            response.Should().BeOfType<ResponseMessageResult>()
                    .Which.Response.Content.ReadAsStringAsync().Result
                    .Should().NotBeNullOrEmpty("becase error message should be provided");
        }

        [Theory]
        [LocksControllerAutoData]
        public void DELETE_returns_correct_result_when_document_lock_was_removed(
            Guid documentId,
            LocksController sut)
        {
            var response = sut.Delete(documentId);

            response.Should().BeOfType<ResponseMessageResult>()
                    .Which.Response.StatusCode
                    .Should().Be(HttpStatusCode.NoContent, "because document locked was removed");
        }
    }
}
