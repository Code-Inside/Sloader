using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
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
            if (viewModel.MasterCrawlerConfigPath == null)
            {
                viewModel.MasterCrawlerConfigIsReadable = false;
            }
            else
            {
                var client = new HttpClient();

                var configString = await client.GetStringAsync(ConfigurationManager.AppSettings[ConfigKeys.MasterCrawlerConfigPath]);

                var deserializer = new Deserializer();
                viewModel.MasterCrawlerConfig = deserializer.Deserialize<MasterCrawlerConfig>(new StringReader(configString));

                var secrets = new MasterCrawlerSecrets();
                secrets.TwitterConsumerKey = ConfigurationManager.AppSettings[ConfigKeys.SecretTwitterConsumerKey];
                secrets.TwitterConsumerSecret = ConfigurationManager.AppSettings[ConfigKeys.SecretTwitterConsumerSecret];

            }


            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

    }
}