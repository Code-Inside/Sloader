using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using Sloader.Crawler;
using Sloader.Crawler.Config;
using Sloader.Results;
using YamlDotNet.Serialization;
using Constants = Sloader.Crawler.Config.Constants;

namespace Sloader.Bootstrapper
{
    public class Program
    {
        public static void Main()
        {
            Trace.TraceInformation("Crawler Console App started.");

            var crawlerResult = InvokeCrawler().Result;
            Trace.TraceInformation("Crawler succeeded - now convert and write to BlobStorage!");

            var json = JsonConvert.SerializeObject(crawlerResult, Constants.CrawlerJsonSerializerSettings);

            var host = new JobHost();
            host.Call(typeof(Program).GetMethod("SaveToAzure"), new { json });
        }

        [NoAutomaticTrigger]
        public static void SaveToAzure([Blob(Constants.SloaderAzureBlobPath)]TextWriter writer, string json)
        {
            writer.Write(json);

            Trace.TraceInformation("And... done.");
        }

        public static async Task<CrawlerRun> InvokeCrawler()
        {
#if DEBUG
            var config = await MasterCrawlerConfigLoader.GetAsync(
                    "https://raw.githubusercontent.com/Code-Inside/Sloader/master/src/Sloader.Web/App_Data/Sloader.yml");
#else
            var config =
                await
                    MasterCrawlerConfigLoader.GetAsync(ConfigurationManager.AppSettings[ConfigKeys.MasterCrawlerConfigPath]);
#endif

            var secrets = new MasterCrawlerSecrets();
            secrets.TwitterConsumerKey = ConfigurationManager.AppSettings[ConfigKeys.SecretTwitterConsumerKey];
            secrets.TwitterConsumerSecret = ConfigurationManager.AppSettings[ConfigKeys.SecretTwitterConsumerSecret];
            var crawler = new MasterCrawler(config, secrets);

            return await crawler.RunAllCrawlers();
        }
    
    }
}
