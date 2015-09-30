using System;
using System.Configuration;
using System.Linq;
using Microsoft.Owin.Hosting;
using Sample.Database;

namespace Sample.Api.ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Installing database");
            Runner.MigrateToLatestVersion(
                ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString);
            Console.WriteLine("Database installed");

            string documentsBaseAddress = "http://localhost:8051/";
            string ticketsBaseAddress = "http://localhost:8052/";
            Console.WriteLine("Starting Documents API server at " + documentsBaseAddress);
            Console.WriteLine("Starting Tickets API server at " + ticketsBaseAddress);
            using (WebApp.Start<Sample.Documents.Api.OwinStartup>(url: documentsBaseAddress))
            using (WebApp.Start<Sample.Tickets.Api.OwinStartup>(url: ticketsBaseAddress))
            {
                Console.WriteLine("Servers started");
                Console.ReadLine(); 
            }
        }
    }
}
