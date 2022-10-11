using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.Management;

namespace GeneralSalesDb
{
    public class Program
    {
        public static void Main(string[] args)
        {

            string bios = "";

            var mbs = new ManagementObjectSearcher("Select SerialNumber From Win32_BIOS");
            var mbslist = mbs.Get();

            foreach (var item in mbslist)
            {
                bios = item["SerialNumber"].ToString();
            }

            //if (bios == "7LBCQW1")
            //if (bios == "5CG44650J3")
            if (bios == "2WD55S2") //halim
            //if (bios == "5CG1127NRR")// jalil
            {
                BuildWebHost(args).Run();
            }



            //string bios = "";

            //var mbs = new ManagementObjectSearcher("Select NumberOfCores From Win32_Processor");
            //var mbslist = mbs.Get();

            //foreach (var item in mbslist)
            //{
            //    bios = item["NumberOfCores"].ToString();
            //}

            ////if (bios == "7LBCQW1")
            ////if (bios == "2WD55S2")
            //if (bios == "2")
            //{
            //    BuildWebHost(args).Run();
            //}

        }

        public static IWebHost BuildWebHost(string[] args) => WebHost.CreateDefaultBuilder(args)
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();


    }
}
