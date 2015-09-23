using Sample.Documents.Database;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sample.Documents.Api.IntegrationTests
{
    /// <summary>
    /// Attribute which indicates a test that uses a database - creates DB before test run and then drops it after
    /// </summary>
    public class UseDatabaseAttribute : BeforeAfterTestAttribute
    {
        private string _connString;

        public UseDatabaseAttribute()
        {
            _connString = ConfigurationManager.ConnectionStrings["DocumentsDBConnectionString"].ConnectionString;
        }

        public override void Before(System.Reflection.MethodInfo methodUnderTest)
        {
            Runner.MigrateToLatestVersion(_connString);

            base.Before(methodUnderTest);
        }

        public override void After(System.Reflection.MethodInfo methodUnderTest)
        {
            Runner.MigrateDown(_connString);

            base.After(methodUnderTest);
        }
    }
}
