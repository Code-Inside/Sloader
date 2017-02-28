using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sloader.Config;
using Sloader.Engine;
using Sloader.Result;

namespace Sloader.Hosts.Console
{
    class Program
    {
        public static void Main(string[] args)
        {
            Trace.TraceInformation("Sloader Console App started.");

            MainAsync(args).GetAwaiter().GetResult();
        }

        public static async Task MainAsync(string[] args)
        {
            await SloaderRunner.AutoRun();
        }
      
    }
}
