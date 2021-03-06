﻿using System;
using System.Data.SqlClient;
using Sample.Api.Shared;
using Sample.Documents.Api.Exceptions;

namespace Sample.Documents.Api.Queries
{
    public class GetDocumentSqlQuery : SqlOperation, IQuery<Guid, DocumentDetails>
    {
        public GetDocumentSqlQuery(string connectionString)
            : base(connectionString)
        {
        }

        public DocumentDetails Execute(Envelope<Guid> request)
        {
            return base.ExecuteReaderOnce<DocumentDetails>(
                "SELECT * FROM [dbo].[Documents] WHERE [Id] = @id",
                reader => new DocumentDetails()
                            {
                                Id = (Guid)reader["Id"],
                                Title = (string)reader["Title"],
                                Content = (string)reader["Content"],
                                CheckedOutBy = reader["CheckedOutBy"] as string
                            },
                () => new DocumentNotFoundException(string.Format("Document {0} was not found", request.Item)),
                new SqlParameter("@id", request.Item));
        }
    }
}
