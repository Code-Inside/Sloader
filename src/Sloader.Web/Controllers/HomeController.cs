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
using Sloader.Crawler.Config;
using Sloader.Web.Models;
using YamlDotNet.Serialization;

namespace Sloader.Web.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            var viewModel = new HomeIndexViewModel();
            viewModel.MasterCrawlerConfigPath = ConfigurationManager.AppSettings[ConfigKeys.MasterCrawlerConfigPath];
            if (viewModel.MasterCrawlerConfigPathIsConfigured)
            {
                var config = await MasterCrawlerConfigLoader.GetAsync(ConfigurationManager.AppSettings[ConfigKeys.MasterCrawlerConfigPath]);

                viewModel.MasterCrawlerConfig = config;

                var secrets = new MasterCrawlerSecrets();
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
                using (var memoryStream = new MemoryStream())
                {
                    await file.DownloadToStreamAsync(memoryStream);
                    viewModel.ResultData = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
                }
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