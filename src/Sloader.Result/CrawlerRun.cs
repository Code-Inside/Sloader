using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sloader.Result
{
    public class CrawlerRun
    {
        public CrawlerRun()
        {
            Data = new Dictionary<string, BaseResult>();
        }
        public DateTime RunOn { get; set; }
        public Dictionary<string, BaseResult> Data { get; set; }
        public long RunDurationInMilliseconds { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}