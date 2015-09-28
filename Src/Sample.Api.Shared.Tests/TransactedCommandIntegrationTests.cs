using System;
using System.Collections.Generic;
using System.Linq;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace Sample.Api.Shared.Tests
{
    public class TransactedCommandIntegrationTests
    {
        class SimpleDataCommand : ICommand<Guid>
        {
            private readonly Action<Guid> _cmd;

            public SimpleDataCommand(Action<Guid> cmd)
            {
                _cmd = cmd;
            }

            public void Execute(Envelope<Guid> input)
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

            var db = Simple.Data.Database.OpenNamedConnection("DBConnectionString");
            db.Documents.Insert(dbDocument);

            var changeTitle = new SimpleDataCommand(id => db.Documents.UpdateById(Id: id, Title: anotherTitle));
            var changeContent = new SimpleDataCommand(id => { throw new InvalidOperationException("second command fails"); });

            var sut = new TransactedCommand<Guid>(
                                new ComposedCommand<Guid>(
                                        changeTitle,
                                        changeContent));

            try
            {
                sut.Execute(new Envelope<Guid>(docId, userName));
            }
            catch (Exception)
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

            var db = Simple.Data.Database.OpenNamedConnection("DBConnectionString");
            db.Documents.Insert(dbDocument);

            var changeTitle = new SimpleDataCommand(id => db.Documents.UpdateById(Id: docId, Title: anotherTitle));
            var changeContent = new SimpleDataCommand(id => db.Documents.UpdateById(Id: docId, Content: anotherContent));

            var sut = new TransactedCommand<Guid>(
                                new ComposedCommand<Guid>(
                                        changeTitle,
                                        changeContent));

            sut.Execute(new Envelope<Guid>(docId, userName));

            var dbDocuments = db.Documents.All();
            Assert.Equal(anotherTitle, dbDocuments.First().Title.ToString());
            Assert.Equal(anotherContent, dbDocuments.First().Content.ToString());
        }
    }
}
