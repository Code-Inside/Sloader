﻿using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using Sloader.Crawler;
using Sloader.Crawler.Config;
using Sloader.Results;

namespace Sloader.Sample.WebJobHost
{
    public class Program
    {
        public static void Main()
        {
            Trace.TraceInformation("Crawler Console App started.");

            var crawlerResult = InvokeCrawler().Result;
            Trace.TraceInformation("Crawler succeeded - now convert and write to BlobStorage!");

            var json = JsonConvert.SerializeObject(crawlerResult);

            var host = new JobHost();
            host.Call(typeof(Program).GetMethod("SaveToAzure"), new { json });
        }

        [NoAutomaticTrigger]
        public static void SaveToAzure([Blob(Constants.SloaderAzureBlobContainer + "/" + Constants.SloaderAzureBlobFileName)]TextWriter writer, string json)
        {
            writer.Write(json);

            Trace.TraceInformation("And... done.");
        }

        public static async Task<CrawlerRun> InvokeCrawler()
        {
#if DEBUG
            var config = await SloaderConfig.Load(
                    "https://raw.githubusercontent.com/Code-Inside/Sloader/master/src/Sloader.Web/App_Data/Sloader.yml");
#else
            var config =
                await
                    SloaderConfig.Load(ConfigurationManager.AppSettings[ConfigKeys.SloaderConfigPath]);
#endif

            var secrets = new SloaderSecrets();
            secrets.TwitterConsumerKey = ConfigurationManager.AppSettings[ConfigKeys.SecretTwitterConsumerKey];
            secrets.TwitterConsumerSecret = ConfigurationManager.AppSettings[ConfigKeys.SecretTwitterConsumerSecret];
            var crawler = new SloaderRunner(config, secrets);

            return await crawler.RunAllCrawlers();
        }

    }
}