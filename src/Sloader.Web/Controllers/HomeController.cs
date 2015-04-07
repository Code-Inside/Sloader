using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Sloader.Crawler.Config;
using Sloader.Results;
using Sloader.Web.Models;
using YamlDotNet.Serialization;

namespace Sloader.Web.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            var viewModel = new HomeIndexViewModel();
            viewModel.MasterCrawlerConfigPath = ConfigurationManager.AppSettings[ConfigKeys.SloaderConfigPath];
            if (viewModel.MasterCrawlerConfigPathIsConfigured)
            {
                var config = await SloaderConfigLoader.GetAsync(ConfigurationManager.AppSettings[ConfigKeys.SloaderConfigPath]);

                viewModel.SloaderConfig = config;

                var secrets = new SloaderSecrets();
                secrets.TwitterConsumerKey = ConfigurationManager.AppSettings[ConfigKeys.SecretTwitterConsumerKey];
                secrets.TwitterConsumerSecret = ConfigurationManager.AppSettings[ConfigKeys.SecretTwitterConsumerSecret];

                viewModel.MasterCrawlerConfigIsTwitterConsumerConfigured = secrets.IsTwitterConsumerConfigured;
            }

            var azureWebJobStorage = ConfigurationManager.AppSettings[ConfigKeys.AzureWebJobStorage];
            if (azureWebJobStorage != null)
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(azureWebJobStorage);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(Constants.SloaderAzureBlobContainer);
                container.CreateIfNotExists();

                var file = await container.GetBlobReferenceFromServerAsync(Constants.SloaderAzureBlobFileName);

                using (var memory = new MemoryStream())
                using (var reader = new StreamReader(memory))
                {
                    await file.DownloadToStreamAsync(memory);

                    memory.Seek(0, SeekOrigin.Begin);

                    viewModel.ResultText = reader.ReadToEnd();
                }

                JsonConverter[] converters = { new CrawlerResultConverter() };
                viewModel.ResultData = JsonConvert.DeserializeObject<CrawlerRun>(viewModel.ResultText, converters);

            }

            return View(viewModel);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

    }
}