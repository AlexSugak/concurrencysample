using Microsoft.Owin.Hosting;
using Sample.Documents.Api;
using Sample.Documents.Database;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Api.ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Installing database");
            Runner.MigrateToLatestVersion(
                ConfigurationManager.ConnectionStrings["DocumentsDBConnectionString"].ConnectionString);
            Console.WriteLine("Database installed");

            string baseAddress = "http://localhost:8051/";
            Console.WriteLine("Starting API server at " + baseAddress);

            // Start OWIN host 
            using (WebApp.Start<OwinStartup>(url: baseAddress))
            {
                Console.WriteLine("Server started");
                Console.ReadLine(); 
            }

        }
    }
}
