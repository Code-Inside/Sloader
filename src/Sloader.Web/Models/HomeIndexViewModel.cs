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
        public bool MasterCrawlerConfigIsReadable { get; set; }
        public MasterCrawlerConfig MasterCrawlerConfig { get; set; }
    }
}