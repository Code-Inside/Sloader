using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Sloader.Result
{
    public class CrawlerRun
    {
        public CrawlerRun()
        {
            Data = new Dictionary<string, BaseResult>();
        }
        public Dictionary<string, BaseResult> Data { get; }

        public DateTime RunOn { get; set; }
        public long RunDurationInMilliseconds { get; set; }

        public void AddResultDataPair(string key, BaseResult data)
        {
            Trace.TraceInformation($"{nameof(AddResultDataPair)} : '{key}'");
            if (Data.ContainsKey(key) == false)
            {
                this.Data.Add(key, data);
            }
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}