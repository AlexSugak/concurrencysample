using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Extensions;
using FluentAssertions;
using Moq;
using System.Web.Http.Results;
using Sample.Documents.Api.Queries;
using Sample.Documents.Api.Commands;
using System.Net.Http;
using Sample.Documents.Api.Controllers;
using Sample.Documents.Api.Exceptions;

namespace Sample.Documents.Api.UnitTests
{
    public class DocumentsControllerTests
    {
        [Theory]
        [MoqAutoData]
        public void get_returns_correct_result_when_no_auth_header_in_request(  
            Mock<IUserNameQuery> userQuery,
            Mock<IGetAllDocumentsQuery> getAllQuery,
            Mock<IGetDocumentQuery> getDocQuery,
            Mock<ICommand<Document>> submitNewCmd,
            Mock<ICommand<Document>> updateCmd)
        {
            var sut = new DocumentsController(
                userQuery.Object, 
                getAllQuery.Object, 
                getDocQuery.Object,
                submitNewCmd.Object, 
                updateCmd.Object);

            var response = sut.Get();

            response.Should().BeOfType<UnauthorizedResult>("because auth header was not specified");
        }

        [Theory]
        [MoqAutoData]
        public void get_returns_200_OK_Result(
            string userName,
            Mock<IUserNameQuery> userQuery, 
            Mock<IGetAllDocumentsQuery> getAllQuery,
            Mock<IGetDocumentQuery> getDocQuery,
            Mock<ICommand<Document>> submitNewCmd,
            Mock<ICommand<Document>> updateCmd)
        {
            userQuery.Setup(q => q.Execute(It.IsAny<HttpRequestMessage>()))
                     .Returns(userName);

            var sut = new DocumentsController(
                userQuery.Object, 
                getAllQuery.Object,
                getDocQuery.Object,
                submitNewCmd.Object, 
                updateCmd.Object);

            var result = sut.Get();

            result
                .Should().NotBeNull()
                .And.BeOfType<OkNegotiatedContentResult<DocumentsModel>>()
                .Which.Content.Documents.Should().BeEmpty();
        }

        [Theory]
        [MoqAutoData]
        public void get_returns_documents_returned_by_query(
            List<DocumentDetails> documents,
            string userName,
            Mock<IUserNameQuery> userQuery, 
            Mock<IGetAllDocumentsQuery> getAllQuery,
            Mock<IGetDocumentQuery> getDocQuery,
            Mock<ICommand<Document>> submitNewCmd,
            Mock<ICommand<Document>> updateCmd)
        {
            getAllQuery.Setup(q => q.Execute())
                       .Returns(documents);
            userQuery.Setup(q => q.Execute(It.IsAny<HttpRequestMessage>()))
                     .Returns(userName);

            var sut = new DocumentsController(
                userQuery.Object, 
                getAllQuery.Object,
                getDocQuery.Object,
                submitNewCmd.Object, 
                updateCmd.Object);

            var result = sut.Get();

            result
                .Should().BeOfType<OkNegotiatedContentResult<DocumentsModel>>()
                .Which.Content.Documents
                                .Select(d => d.Title).Should().Equal(documents.Select(d => d.Title));
        }

        [Theory]
        [MoqAutoData]
        public void post_returns_correct_result_when_no_auth_header_in_request(
            DocumentModel document,
            Mock<IUserNameQuery> userQuery,
            Mock<IGetAllDocumentsQuery> getAllQuery,
            Mock<IGetDocumentQuery> getDocQuery,
            Mock<ICommand<Document>> submitNewCmd,
            Mock<ICommand<Document>> updateCmd)
        {
            var sut = new DocumentsController(
                userQuery.Object, 
                getAllQuery.Object,
                getDocQuery.Object,
                submitNewCmd.Object, 
                updateCmd.Object);

            var response = sut.Post(document);

            response.Should().BeOfType<UnauthorizedResult>("because auth header was not specified");
        }

        [Theory]
        [MoqAutoData]
        public void post_returns_201_Created_when_command_succeeds(
            DocumentModel document,
            string userName,
            Mock<IUserNameQuery> userQuery, 
            Mock<IGetAllDocumentsQuery> getAllQuery,
            Mock<IGetDocumentQuery> getDocQuery,
            Mock<ICommand<Document>> submitNewCmd,
            Mock<ICommand<Document>> updateCmd)
        {
            userQuery.Setup(q => q.Execute(It.IsAny<HttpRequestMessage>()))
                     .Returns(userName);

            var sut = new DocumentsController(
                userQuery.Object, 
                getAllQuery.Object,
                getDocQuery.Object,
                submitNewCmd.Object, 
                updateCmd.Object);

            var result = sut.Post(document);

            result.Should().BeOfType<CreatedNegotiatedContentResult<DocumentResponseModel>>()
                .Which.Content.ShouldBeEquivalentTo(document, options => options.ExcludingMissingMembers());
        }

        [Theory]
        [MoqAutoData]
        public void post_returns_400_BadRequest_on_validation_exception(
            DocumentModel document,
            ValidationException exception,
            string userName,
            Mock<IUserNameQuery> userQuery, 
            Mock<IGetAllDocumentsQuery> getAllQuery,
            Mock<IGetDocumentQuery> getDocQuery,
            Mock<ICommand<Document>> submitNewCmd,
            Mock<ICommand<Document>> updateCmd)
        {
            submitNewCmd.Setup(c => c.Execute(It.IsAny<Envelope<Document>>()))
                        .Throws(exception);
            userQuery.Setup(q => q.Execute(It.IsAny<HttpRequestMessage>()))
                     .Returns(userName);

            var sut = new DocumentsController(
                userQuery.Object, 
                getAllQuery.Object,
                getDocQuery.Object,
                submitNewCmd.Object, 
                updateCmd.Object);

            var result = sut.Post(document);

            result.Should().BeOfType<BadRequestErrorMessageResult>()
                .Which.Message.Should().Be(exception.Message);
        }

        [Theory]
        [MoqAutoData]
        public void put_returns_409_Conflict_on_lock_exception(
            Guid documentId,
            DocumentModel document,
            DocumentLockedException exception,
            string userName,
            Mock<IUserNameQuery> userQuery,
            Mock<IGetAllDocumentsQuery> getAllQuery,
            Mock<IGetDocumentQuery> getDocQuery,
            Mock<ICommand<Document>> submitNewCmd,
            Mock<ICommand<Document>> updateCmd)
        {
            updateCmd.Setup(c => c.Execute(It.IsAny<Envelope<Document>>()))
                     .Throws(exception);
            userQuery.Setup(q => q.Execute(It.IsAny<HttpRequestMessage>()))
                     .Returns(userName);

            var sut = new DocumentsController(
                userQuery.Object, 
                getAllQuery.Object,
                getDocQuery.Object,
                submitNewCmd.Object, 
                updateCmd.Object);

            var result = sut.Put(document, documentId);

            result.Should().BeOfType<ConflictResult>("because document lock exception was thrown");
        }

        [Theory]
        [MoqAutoData]
        public void getById_returns_document_returned_by_query(
            DocumentDetails document,
            string userName,
            Mock<IUserNameQuery> userQuery,
            Mock<IGetAllDocumentsQuery> getAllQuery,
            Mock<IGetDocumentQuery> getDocQuery,
            Mock<ICommand<Document>> submitNewCmd,
            Mock<ICommand<Document>> updateCmd)
        {
            getDocQuery.Setup(q => q.Execute(document.Id))
                       .Returns(document);
            userQuery.Setup(q => q.Execute(It.IsAny<HttpRequestMessage>()))
                     .Returns(userName);

            var sut = new DocumentsController(
                userQuery.Object,
                getAllQuery.Object,
                getDocQuery.Object,
                submitNewCmd.Object,
                updateCmd.Object);

            var result = sut.GetById(document.Id);

            result
                .Should().BeOfType<OkNegotiatedContentResult<DocumentResponseModel>>()
                .Which.Content.Should().ShouldBeEquivalentTo(document, options => options.ExcludingMissingMembers());
        }

        [Theory]
        [MoqAutoData]
        public void getById_returns_404_NotFound_when_document_not_found(
            DocumentDetails document,
            string userName,
            Mock<IUserNameQuery> userQuery,
            Mock<IGetAllDocumentsQuery> getAllQuery,
            Mock<IGetDocumentQuery> getDocQuery,
            Mock<ICommand<Document>> submitNewCmd,
            Mock<ICommand<Document>> updateCmd)
        {
            getDocQuery.Setup(q => q.Execute(document.Id))
                       .Throws<DocumentNotFoundException>();
            userQuery.Setup(q => q.Execute(It.IsAny<HttpRequestMessage>()))
                     .Returns(userName);

            var sut = new DocumentsController(
                userQuery.Object,
                getAllQuery.Object,
                getDocQuery.Object,
                submitNewCmd.Object,
                updateCmd.Object);

            var result = sut.GetById(document.Id);

            result.Should().BeOfType<NotFoundResult>("because document not found exception is thrown");
        }
    }
}
