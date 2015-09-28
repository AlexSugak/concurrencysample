using System;
using System.Configuration;
using Sample.Database;
using Xunit;
using System.Data.SqlClient;

namespace Sample.Api.Shared.Tests
{
    /// <summary>
    /// Attribute which indicates a test that uses a database - creates DB before test run and then drops it after
    /// </summary>
    public class UseDatabaseAttribute : BeforeAfterTestAttribute
    {
        private string _connString;

        public UseDatabaseAttribute()
        {
            _connString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
        }

        public override void Before(System.Reflection.MethodInfo methodUnderTest)
        {
            Runner.MigrateToLatestVersion(_connString);

            base.Before(methodUnderTest);
        }

        public override void After(System.Reflection.MethodInfo methodUnderTest)
        {
            Runner.MigrateDownToCleanDb(_connString);

            base.After(methodUnderTest);
        }
    }
}
