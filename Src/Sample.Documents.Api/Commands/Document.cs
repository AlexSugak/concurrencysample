using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Documents.Api.Commands
{
    public interface IDocumentReference
    {
        Guid DocumentId { get; }
    }

    public class DocumentReference : IDocumentReference
    {
        public DocumentReference(Guid id)
        {
            DocumentId = id;
        }

        public Guid DocumentId { get; private set; }
    }

    public class Document : IDocumentReference
    {
        public Document(Guid id, string title, string content)
        {
            DocumentId = id;
            Title = title;
            Content = content;
        }

        public Guid DocumentId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
