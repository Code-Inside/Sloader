using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sloader.Web.Models
{
    public class HomeIndexViewModel
    {
        public string MasterCrawlerConfigPath { get; set; }
        public bool MasterCrawlerConfigIsReadable { get; set; }

    }
}