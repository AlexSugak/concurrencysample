using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Documents.Database
{
    [Migration(201509222320)]
    public class Mig_201509222320_CreateInitialSchema : Migration
    {
        public override void Down()
        {
            Delete.Table("Documents").InSchema("dbo");
        }

        public override void Up()
        {
            Create.Table("Documents").InSchema("dbo")
                    .WithColumn("Id").AsGuid().PrimaryKey()
                    .WithColumn("Title").AsString(100).NotNullable()
                    .WithColumn("Content").AsString(int.MaxValue).NotNullable()
                    .WithColumn("CheckedOutBy").AsString(100).Nullable();
        }
    }
}
