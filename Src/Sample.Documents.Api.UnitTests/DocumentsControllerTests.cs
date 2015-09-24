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

namespace Sample.Documents.Api.UnitTests
{
    public class DocumentsControllerTests
    {
        [Theory]
        [MoqAutoData]
        public void get_returns_correct_result_when_no_auth_header_in_request(  
            Mock<IUserNameQuery> userQuery, 
            Mock<IGetAllDocumentsQuery> getAllQuery,
            Mock<ISubmitNewDocumentCommand> submitNewCmd)
        {
            var sut = new DocumentsController(userQuery.Object, getAllQuery.Object, submitNewCmd.Object);

            var response = sut.Get();

            response.Should().BeOfType<UnauthorizedResult>("because auth header was not specified");
        }

        [Theory]
        [MoqAutoData]
        public void get_returns_200_OK_Result(
            string userName,
            Mock<IUserNameQuery> userQuery, 
            Mock<IGetAllDocumentsQuery> getAllQuery, 
            Mock<ISubmitNewDocumentCommand> submitNewCmd)
        {
            userQuery.Setup(q => q.Execute(It.IsAny<HttpRequestMessage>()))
                     .Returns(userName);
            var sut = new DocumentsController(userQuery.Object, getAllQuery.Object, submitNewCmd.Object);

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
            Mock<ISubmitNewDocumentCommand> submitNewCmd)
        {
            getAllQuery.Setup(q => q.Execute())
                       .Returns(documents);
            userQuery.Setup(q => q.Execute(It.IsAny<HttpRequestMessage>()))
                     .Returns(userName);
            var sut = new DocumentsController(userQuery.Object, getAllQuery.Object, submitNewCmd.Object);

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
            Mock<ISubmitNewDocumentCommand> submitNewCmd)
        {
            var sut = new DocumentsController(userQuery.Object, getAllQuery.Object, submitNewCmd.Object);

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
            Mock<ISubmitNewDocumentCommand> submitNewCmd)
        {
            userQuery.Setup(q => q.Execute(It.IsAny<HttpRequestMessage>()))
                     .Returns(userName);
            var sut = new DocumentsController(userQuery.Object, getAllQuery.Object, submitNewCmd.Object);

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
            Mock<ISubmitNewDocumentCommand> submitNewCmd)
        {
            submitNewCmd.Setup(c => c.Execute(It.IsAny<NewDocument>()))
                        .Throws(exception);
            userQuery.Setup(q => q.Execute(It.IsAny<HttpRequestMessage>()))
                     .Returns(userName);
            var sut = new DocumentsController(userQuery.Object, getAllQuery.Object, submitNewCmd.Object);

            var result = sut.Post(document);

            result.Should().BeOfType<BadRequestErrorMessageResult>()
                .Which.Message.Should().Be(exception.Message);
        }
    }
}
