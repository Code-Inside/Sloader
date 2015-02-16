using System;
using System.Collections.Generic;

namespace Sloader.Results
{
    public class CrawlerRun
    {
        public CrawlerRun()
        {
            Results = new List<BaseCrawlerResult>();
        }
        public DateTime RunOn { get; set; }
        public List<BaseCrawlerResult> Results { get; set; }
        public long RunDurationInMilliseconds { get; set; }
    }
}