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
        static void Main(string[] args)
        {
            Trace.TraceInformation("Crawler Console App started.");

            var crawlerResult = InvokeCrawler().Result;
            Trace.TraceInformation("Crawler succeeded - now convert and write to BlobStorage!");

            var json = JsonConvert.SerializeObject(crawlerResult);

            System.IO.File.WriteAllText("Sloader.json", json);
        }

        public static async Task<CrawlerRun> InvokeCrawler()
        {
            // possible: "https://raw.githubusercontent.com/Code-Inside/Sloader/master/src/Sloader.Web/App_Data/Sloader.yml";
            var config = await SloaderConfig.Load(ConfigurationManager.AppSettings[FixedConfigKeys.SloaderConfigPath], ConfigurationManager.AppSettings.AllKeys.ToDictionary(k => k, v => ConfigurationManager.AppSettings[v]));
            
            var runner = new SloaderRunner(config);

            var crawlerRun = await runner.RunAllCrawlers();

            return crawlerRun;
        }
    }
}
