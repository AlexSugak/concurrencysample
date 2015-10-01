using System;

namespace Sample.Documents.Api.Commands
{
    public class LockInfo : IDocumentReference
    {
        public LockInfo(Guid documentId)
        {
            DocumentId = documentId;
        }

        public Guid DocumentId { get; set; }
    }
}
