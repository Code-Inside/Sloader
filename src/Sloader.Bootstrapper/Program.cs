using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using Sloader.Crawler;
using Sloader.Shared;
using Sloader.Types;
using Constants = Sloader.Crawler.Constants;

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
        public static void SaveToAzure([Blob("sloader/data.json")]TextWriter writer, string json)
        {
            writer.Write(json);

            Trace.TraceInformation("And... done.");
        }

        public static async Task<CrawlerRun> InvokeCrawler()
        {
            var client = new HttpClient();
#if DEBUG
            var configString = await client.GetStringAsync("https://raw.githubusercontent.com/Code-Inside/Hub/master/CrawlerConfig.json");
#else
            var configString = await client.GetStringAsync(ConfigurationManager.AppSettings[ConfigKeys.MasterCrawlerConfigPath]);
#endif
            var config = JsonConvert.DeserializeObject<MasterCrawlerConfig>(configString);

            var secrets = new MasterCrawlerSecrets();
            secrets.TwitterConsumerKey = ConfigurationManager.AppSettings[ConfigKeys.SecretTwitterConsumerKey];
            secrets.TwitterConsumerSecret = ConfigurationManager.AppSettings[ConfigKeys.SecretTwitterConsumerSecret];
            var crawler = new MasterCrawler(config, secrets);

            return await crawler.RunAllCrawlers();
        }
    
    }
}
