using Ploeh.AutoFixture.Xunit;
using Sample.Documents.Api.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;

namespace Sample.Documents.Api.IntegrationTests
{
    public class TransactedCommandTests
    {
        class SimpleDataCommand : ICommand<DocumentReference>
        {
            private readonly Action<DocumentReference> _cmd;

            public SimpleDataCommand(Action<DocumentReference> cmd)
            {
                _cmd = cmd;
            }

            public void Execute(Envelope<DocumentReference> input)
            {
                _cmd(input.Item);
            }
        }

        [Theory]
        [AutoData]
        [UseDatabase]
        public void must_rollback_db_changes_from_first_inner_cmd_if_second_inner_cmd_failes(
            Guid docId,
            string userName,
            string title,
            string content,
            string anotherTitle)
        {
            var dbDocument = new
            {
                Id = docId,
                Title = title,
                Content = content,
                CheckedOutBy = userName
            };

            var db = Simple.Data.Database.OpenNamedConnection("DocumentsDBConnectionString");
            db.Documents.Insert(dbDocument);

            var changeTitle = new SimpleDataCommand(id => db.Documents.UpdateById(Id: docId, Title: anotherTitle));
            var changeContent = new SimpleDataCommand(id => { throw new InvalidOperationException("second command fails"); });

            var sut = new TransactedCommand<DocumentReference>(
                                new ComposedCommand<DocumentReference>(
                                        changeTitle, 
                                        changeContent));

            try
            {
                sut.Execute(new Envelope<DocumentReference>(new DocumentReference(docId), userName));
            }
            catch(Exception)
            { }

            var dbDocuments = db.Documents.All();
            Assert.Equal(title, dbDocuments.First().Title.ToString());
        }

        [Theory]
        [AutoData]
        [UseDatabase]
        public void must_save_db_changes_from_both_inner_commands(
            Guid docId,
            string userName,
            string title,
            string content,
            string anotherTitle,
            string anotherContent)
        {
            var dbDocument = new
            {
                Id = docId,
                Title = title,
                Content = content,
                CheckedOutBy = userName
            };

            var db = Simple.Data.Database.OpenNamedConnection("DocumentsDBConnectionString");
            db.Documents.Insert(dbDocument);

            var changeTitle = new SimpleDataCommand(id => db.Documents.UpdateById(Id: docId, Title: anotherTitle));
            var changeContent = new SimpleDataCommand(id => db.Documents.UpdateById(Id: docId, Content: anotherContent));

            var sut = new TransactedCommand<DocumentReference>(
                                new ComposedCommand<DocumentReference>(
                                        changeTitle,
                                        changeContent));

            sut.Execute(new Envelope<DocumentReference>(new DocumentReference(docId), userName));

            var dbDocuments = db.Documents.All();
            Assert.Equal(anotherTitle, dbDocuments.First().Title.ToString());
            Assert.Equal(anotherContent, dbDocuments.First().Content.ToString());
        }
    }
}
