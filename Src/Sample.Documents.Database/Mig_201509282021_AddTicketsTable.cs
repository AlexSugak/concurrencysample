using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Documents.Database
{
    [Migration(201509282021)]
    public class Mig_201509282021_AddTicketsTable : Migration
    {
        public override void Down()
        {
            Delete.Table("Tickets").InSchema("dbo");
        }

        public override void Up()
        {
            Create.Table("Tickets").InSchema("dbo")
                    .WithColumn("Id").AsGuid().PrimaryKey()
                    .WithColumn("Title").AsString(100).NotNullable()
                    .WithColumn("Description").AsString(500).Nullable()
                    .WithColumn("Severity").AsString(100).NotNullable()
                    .WithColumn("Status").AsString(50).NotNullable()
                    .WithColumn("AssignedTo").AsString(100).Nullable()
                    .WithColumn("Version").AsGuid().NotNullable();
        }
    }
}
