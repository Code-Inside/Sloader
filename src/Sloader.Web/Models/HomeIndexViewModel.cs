using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sloader.Crawler.Config;

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

        public bool MasterCrawlerConfigIsReadable { get; set; }
        public MasterCrawlerConfig MasterCrawlerConfig { get; set; }
        public bool MasterCrawlerConfigIsTwitterConsumerConfigured { get; set; }
        public string MasterCrawlerRawConfig { get; set; }
    }
}