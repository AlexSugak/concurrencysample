using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Database
{
    [Migration(201509291922)]
    public class Mig_201509291922_ChangeTicketVersionToTimestamp : Migration
    {
        public override void Down()
        {
            Delete.Column("Version").FromTable("Tickets").InSchema("dbo");
            Alter.Table("Tickets").InSchema("dbo").AddColumn("Version").AsGuid().Nullable();
        }

        public override void Up()
        {
            Delete.Column("Version").FromTable("Tickets").InSchema("dbo");
            Alter.Table("Tickets").InSchema("dbo").AddColumn("Version").AsCustom("rowversion");
        }
    }
}
