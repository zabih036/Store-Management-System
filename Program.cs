using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.Management;

namespace GeneralSalesDb
{
    public class Program
    {
        public static void Main(string[] args)
        {
                BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) => WebHost.CreateDefaultBuilder(args)
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

    }
}
