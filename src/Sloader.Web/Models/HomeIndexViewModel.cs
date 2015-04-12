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
        public string SloaderConfigPath { get; set; }

        public bool SloaderConfigPathIsConfigured
        {
            get
            {
                if (string.IsNullOrWhiteSpace(SloaderConfigPath))
                    return false;

                return true;
            }
        }

        public bool SloaderConfigIsValid
        {
            get
            {
                if (SloaderConfig == null)
                    return false;

                return true;
            }
        }

        public SloaderConfig SloaderConfig { get; set; }
        public bool SloaderConfigIsTwitterConsumerConfigured { get; set; }
        public CrawlerRun ResultData { get; set; }
        public string ResultText { get; set; }
    }
}