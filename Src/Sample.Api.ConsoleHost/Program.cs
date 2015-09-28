using System;
using System.Configuration;
using System.Linq;
using Microsoft.Owin.Hosting;
using Sample.Database;
using Sample.Documents.Api;

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
            using (WebApp.Start<OwinStartup>(url: baseAddress))
            {
                Console.WriteLine("Server started");
                Console.ReadLine(); 
            }
        }
    }
}
