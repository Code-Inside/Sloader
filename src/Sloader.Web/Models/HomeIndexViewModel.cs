using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sloader.Crawler.Config;
using Sloader.Results;

namespace Sloader.Web.Models
{
    public class HomeIndexViewModel
    {
        public string MasterCrawlerConfigPath { get; set; }

        public bool MasterCrawlerConfigPathIsConfigured
        {
            get
            {
                if (string.IsNullOrWhiteSpace(MasterCrawlerConfigPath))
                    return false;

                return true;
            }
        }

        public bool MasterCrawlerConfigIsValid
        {
            get
            {
                if (SloaderConfig == null)
                    return false;

                return true;
            }
        }

        public SloaderConfig SloaderConfig { get; set; }
        public bool MasterCrawlerConfigIsTwitterConsumerConfigured { get; set; }
        public CrawlerRun ResultData { get; set; }
        public string ResultText { get; set; }
    }
}