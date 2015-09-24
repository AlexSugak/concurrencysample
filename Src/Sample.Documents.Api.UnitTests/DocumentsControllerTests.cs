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

namespace Sample.Documents.Api.UnitTests
{
    public class DocumentsControllerTests
    {
        [Theory]
        [MoqAutoData]
        public void get_returns_200_OK_Result(
            Mock<IGetAllDocumentsQuery> getAllQuery, 
            Mock<ISubmitNewDocumentCommand> submitNewCmd)
        {
            var sut = new DocumentsController(getAllQuery.Object, submitNewCmd.Object);

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
            Mock<IGetAllDocumentsQuery> getAllQuery,
            Mock<ISubmitNewDocumentCommand> submitNewCmd)
        {
            getAllQuery.Setup(q => q.Execute()).Returns(documents);
            var sut = new DocumentsController(getAllQuery.Object, submitNewCmd.Object);

            var result = sut.Get();

            result
                .Should().BeOfType<OkNegotiatedContentResult<DocumentsModel>>()
                .Which.Content.Documents
                                .Select(d => d.Title).Should().Equal(documents.Select(d => d.Title));
        }

        [Theory]
        [MoqAutoData]
        public void post_returns_201_Created_when_command_succeeds(
            DocumentModel document,
            Mock<IGetAllDocumentsQuery> getAllQuery,
            Mock<ISubmitNewDocumentCommand> submitNewCmd)
        {
            var sut = new DocumentsController(getAllQuery.Object, submitNewCmd.Object);

            var result = sut.Post(document);

            result.Should().BeOfType<CreatedNegotiatedContentResult<DocumentResponseModel>>()
                .Which.Content.ShouldBeEquivalentTo(document, options => options.ExcludingMissingMembers());
        }

        [Theory]
        [MoqAutoData]
        public void post_returns_400_BadRequest_on_validation_exception(
            DocumentModel document,
            ValidationException exception,
            Mock<IGetAllDocumentsQuery> getAllQuery,
            Mock<ISubmitNewDocumentCommand> submitNewCmd)
        {
            submitNewCmd.Setup(c => c.Execute(It.IsAny<NewDocument>())).Throws(exception);
            var sut = new DocumentsController(getAllQuery.Object, submitNewCmd.Object);

            var result = sut.Post(document);

            result.Should().BeOfType<BadRequestErrorMessageResult>()
                .Which.Message.Should().Be(exception.Message);
        }
    }
}
