using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Results;
using FluentAssertions;
using Moq;
using Ploeh.AutoFixture.Xunit;
using Sample.Api.Shared;
using Sample.Documents.Api.Queries;
using Sample.Documents.Api.Commands;
using Sample.Documents.Api.Controllers;
using Sample.Documents.Api.Exceptions;
using Xunit.Extensions;
using System.Web.Http.Routing;

namespace Sample.Documents.Api.UnitTests
{
    public class DocumentsControllerTests
    {
        [Theory]
        [DocumentsControllerAutoData]
        public void get_returns_correct_result_on_unauthorized(  
            [Frozen]Mock<IQuery<EmptyRequest, IEnumerable<DocumentDetails>>> query,
            DocumentsController sut)
        {
            query.Setup(q => q.Execute(It.IsAny<Envelope<EmptyRequest>>())).Throws<UnauthorizedAccessException>();

            var response = sut.Get();

            response.Should().BeOfType<UnauthorizedResult>("because auth header was not specified");
        }

        [Theory]
        [DocumentsControllerAutoData]
        public void get_returns_200_OK_Result(
            DocumentsController sut)
        {
            var result = sut.Get();

            result
                .Should().NotBeNull()
                .And.BeOfType<OkNegotiatedContentResult<DocumentsModel>>()
                .Which.Content.Documents.Should().BeEmpty();
        }

        [Theory]
        [DocumentsControllerAutoData]
        public void get_returns_documents_returned_by_query(
            List<DocumentDetails> documents,
            [Frozen]Mock<IQuery<EmptyRequest, IEnumerable<DocumentDetails>>> getAllQuery,
            DocumentsController sut)
        {
            getAllQuery.Setup(q => q.Execute(It.IsAny<Envelope<EmptyRequest>>()))
                       .Returns(documents);

            var result = sut.Get();

            result
                .Should().BeOfType<OkNegotiatedContentResult<DocumentsModel>>()
                .Which.Content.Documents
                                .Select(d => d.Title).Should().Equal(documents.Select(d => d.Title));
        }

        [Theory]
        [DocumentsControllerAutoData]
        public void post_returns_correct_result_when_unauthorized(
            DocumentModel document,
            [Frozen]Mock<ICommand<Document>> cmd,
            DocumentsController sut)
        {
            cmd.Setup(c => c.Execute(It.IsAny<Envelope<Document>>())).Throws<UnauthorizedAccessException>();

            var response = sut.Post(document);

            response.Should().BeOfType<UnauthorizedResult>("because auth header was not specified");
        }

        [Theory]
        [DocumentsControllerAutoData]
        public void post_returns_201_Created_when_command_succeeds(
            DocumentModel document,
            [Frozen]Mock<UrlHelper> url,
            DocumentsController sut)
        {
            var createdUri = "http://localhost:8051/api/documents/123";
            url.Setup(u => u.Link(It.IsAny<string>(), It.IsAny<object>()))
                            .Returns(createdUri);

            var result = sut.Post(document);

            result.Should().BeOfType<CreatedNegotiatedContentResult<DocumentResponseModel>>()
                .Which.Content.ShouldBeEquivalentTo(document, options => options.ExcludingMissingMembers());
        }

        [Theory]
        [DocumentsControllerAutoData]
        public void post_returns_400_BadRequest_on_validation_exception(
            DocumentModel document,
            ValidationException exception,
            [Frozen]Mock<ICommand<Document>> submitNewCmd,
            DocumentsController sut)
        {
            submitNewCmd.Setup(c => c.Execute(It.IsAny<Envelope<Document>>()))
                        .Throws(exception);

            var result = sut.Post(document);

            result.Should().BeOfType<BadRequestErrorMessageResult>()
                .Which.Message.Should().Be(exception.Message);
        }

        [Theory]
        [DocumentsControllerAutoData]
        public void put_returns_409_Conflict_on_lock_exception(
            Guid documentId,
            DocumentModel document,
            DocumentLockedException exception,
            [Frozen]Mock<ICommand<Document>> updateCmd,
            DocumentsController sut)
        {
            updateCmd.Setup(c => c.Execute(It.IsAny<Envelope<Document>>()))
                     .Throws(exception);

            var result = sut.Put(document, documentId);

            result.Should().BeOfType<ResponseMessageResult>()
                  .Which.Response.StatusCode.Should().Be(HttpStatusCode.Conflict, "because document lock exception was thrown");

            result.Should().BeOfType<ResponseMessageResult>()
                  .Which.Response.Content.ReadAsStringAsync().Result.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [DocumentsControllerAutoData]
        public void getById_returns_document_returned_by_query(
            DocumentDetails document,
            [Frozen]Mock<IQuery<Guid, DocumentDetails>> getDocQuery,
            DocumentsController sut)
        {
            getDocQuery.Setup(q => q.Execute(It.Is<Envelope<Guid>>(r => r.Item == document.Id)))
                       .Returns(document);

            var result = sut.GetById(document.Id);

            result
                .Should().BeOfType<OkNegotiatedContentResult<DocumentResponseModel>>()
                .Which.Content.Should().ShouldBeEquivalentTo(document, options => options.ExcludingMissingMembers());
        }

        [Theory]
        [DocumentsControllerAutoData]
        public void getById_returns_404_NotFound_when_document_not_found(
            DocumentDetails document,
            [Frozen]Mock<IQuery<Guid, DocumentDetails>> getDocQuery,
            DocumentsController sut)
        {
            getDocQuery.Setup(q => q.Execute(It.Is<Envelope<Guid>>(r => r.Item == document.Id)))
                       .Throws<DocumentNotFoundException>();

            var result = sut.GetById(document.Id);

            result.Should().BeOfType<NotFoundResult>("because document not found exception is thrown");
        }

        [Theory]
        [DocumentsControllerAutoData]
        public void getVyId_returns_correct_result_when_unauthorized(
            Guid documentId,
            [Frozen]Mock<IQuery<Guid, DocumentDetails>> query,
            DocumentsController sut)
        {
            query.Setup(q => q.Execute(It.IsAny<Envelope<Guid>>())).Throws<UnauthorizedAccessException>();

            var response = sut.GetById(documentId);

            response.Should().BeOfType<UnauthorizedResult>("because auth header was not specified");
        }

        [Theory]
        [DocumentsControllerAutoData]
        public void delete_returns_409_Conflict_on_lock_exception(
            Guid documentId,
            DocumentModel document,
            DocumentLockedException exception,
            [Frozen]Mock<ICommand<DocumentReference>> deleteCmd,
            DocumentsController sut)
        {
            deleteCmd.Setup(c => c.Execute(It.IsAny<Envelope<DocumentReference>>()))
                     .Throws(exception);

            var result = sut.Delete(documentId);

            result.Should().BeOfType<ResponseMessageResult>()
                  .Which.Response.StatusCode.Should().Be(HttpStatusCode.Conflict, "because document lock exception was thrown");

            result.Should().BeOfType<ResponseMessageResult>()
                  .Which.Response.Content.ReadAsStringAsync().Result.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [DocumentsControllerAutoData]
        public void delete_returns_409_NoContent_whend_document_deleted(
            Guid documentId,
            DocumentModel document,
            DocumentLockedException exception,
            DocumentsController sut)
        {
            var result = sut.Delete(documentId);

            result.Should().BeOfType<ResponseMessageResult>()
                  .Which.Response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
