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

namespace Sample.Documents.Api.UnitTests
{
    public class DocumentsControllerTests
    {
        [Theory]
        [DocumentsControllerAutoData]
        public void get_returns_correct_result_when_no_auth_header_in_request(  
            DocumentsController sut)
        {
            var response = sut.Get();

            response.Should().BeOfType<UnauthorizedResult>("because auth header was not specified");
        }

        [Theory]
        [DocumentsControllerAutoData]
        public void get_returns_200_OK_Result(
            string userName,
            [Frozen]Mock<IUserNameQuery> userQuery, 
            DocumentsController sut)
        {
            userQuery.Setup(q => q.Execute(It.IsAny<HttpRequestMessage>()))
                     .Returns(userName);

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
            string userName,
            [Frozen]Mock<IUserNameQuery> userQuery,
            [Frozen]Mock<IGetAllDocumentsQuery> getAllQuery,
            DocumentsController sut)
        {
            getAllQuery.Setup(q => q.Execute())
                       .Returns(documents);
            userQuery.Setup(q => q.Execute(It.IsAny<HttpRequestMessage>()))
                     .Returns(userName);

            var result = sut.Get();

            result
                .Should().BeOfType<OkNegotiatedContentResult<DocumentsModel>>()
                .Which.Content.Documents
                                .Select(d => d.Title).Should().Equal(documents.Select(d => d.Title));
        }

        [Theory]
        [DocumentsControllerAutoData]
        public void post_returns_correct_result_when_no_auth_header_in_request(
            DocumentModel document,
            DocumentsController sut)
        {
            var response = sut.Post(document);

            response.Should().BeOfType<UnauthorizedResult>("because auth header was not specified");
        }

        [Theory]
        [DocumentsControllerAutoData]
        public void post_returns_201_Created_when_command_succeeds(
            DocumentModel document,
            string userName,
            [Frozen]Mock<IUserNameQuery> userQuery, 
            DocumentsController sut)
        {
            userQuery.Setup(q => q.Execute(It.IsAny<HttpRequestMessage>()))
                     .Returns(userName);

            var result = sut.Post(document);

            result.Should().BeOfType<CreatedNegotiatedContentResult<DocumentResponseModel>>()
                .Which.Content.ShouldBeEquivalentTo(document, options => options.ExcludingMissingMembers());
        }

        [Theory]
        [DocumentsControllerAutoData]
        public void post_returns_400_BadRequest_on_validation_exception(
            DocumentModel document,
            ValidationException exception,
            string userName,
            [Frozen]Mock<IUserNameQuery> userQuery, 
            [Frozen]Mock<ICommand<Document>> submitNewCmd,
            DocumentsController sut)
        {
            submitNewCmd.Setup(c => c.Execute(It.IsAny<Envelope<Document>>()))
                        .Throws(exception);
            userQuery.Setup(q => q.Execute(It.IsAny<HttpRequestMessage>()))
                     .Returns(userName);

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
            string userName,
            [Frozen]Mock<IUserNameQuery> userQuery,
            [Frozen]Mock<ICommand<Document>> updateCmd,
            DocumentsController sut)
        {
            updateCmd.Setup(c => c.Execute(It.IsAny<Envelope<Document>>()))
                     .Throws(exception);
            userQuery.Setup(q => q.Execute(It.IsAny<HttpRequestMessage>()))
                     .Returns(userName);

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
            string userName,
            [Frozen]Mock<IUserNameQuery> userQuery,
            [Frozen]Mock<IGetDocumentQuery> getDocQuery,
            DocumentsController sut)
        {
            getDocQuery.Setup(q => q.Execute(document.Id))
                       .Returns(document);
            userQuery.Setup(q => q.Execute(It.IsAny<HttpRequestMessage>()))
                     .Returns(userName);

            var result = sut.GetById(document.Id);

            result
                .Should().BeOfType<OkNegotiatedContentResult<DocumentResponseModel>>()
                .Which.Content.Should().ShouldBeEquivalentTo(document, options => options.ExcludingMissingMembers());
        }

        [Theory]
        [DocumentsControllerAutoData]
        public void getById_returns_404_NotFound_when_document_not_found(
            DocumentDetails document,
            string userName,
            [Frozen]Mock<IUserNameQuery> userQuery,
            [Frozen]Mock<IGetDocumentQuery> getDocQuery,
            DocumentsController sut)
        {
            getDocQuery.Setup(q => q.Execute(document.Id))
                       .Throws<DocumentNotFoundException>();
            userQuery.Setup(q => q.Execute(It.IsAny<HttpRequestMessage>()))
                     .Returns(userName);

            var result = sut.GetById(document.Id);

            result.Should().BeOfType<NotFoundResult>("because document not found exception is thrown");
        }

        [Theory]
        [DocumentsControllerAutoData]
        public void getVyId_returns_correct_result_when_no_auth_header_in_request(
            Guid documentId,
            DocumentsController sut)
        {
            var response = sut.GetById(documentId);

            response.Should().BeOfType<UnauthorizedResult>("because auth header was not specified");
        }

        [Theory]
        [DocumentsControllerAutoData]
        public void delete_returns_409_Conflict_on_lock_exception(
            Guid documentId,
            DocumentModel document,
            DocumentLockedException exception,
            string userName,
            [Frozen]Mock<IUserNameQuery> userQuery,
            [Frozen]Mock<ICommand<DocumentReference>> deleteCmd,
            DocumentsController sut)
        {
            deleteCmd.Setup(c => c.Execute(It.IsAny<Envelope<DocumentReference>>()))
                     .Throws(exception);
            userQuery.Setup(q => q.Execute(It.IsAny<HttpRequestMessage>()))
                     .Returns(userName);

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
            string userName,
            [Frozen]Mock<IUserNameQuery> userQuery,
            DocumentsController sut)
        {
            userQuery.Setup(q => q.Execute(It.IsAny<HttpRequestMessage>()))
                     .Returns(userName);

            var result = sut.Delete(documentId);

            result.Should().BeOfType<ResponseMessageResult>()
                  .Which.Response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
