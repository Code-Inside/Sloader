using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Sloader.Result
{
    /// <summary>
    /// Container class which stores some common "run" data and the actual result collection.
    /// </summary>
    public class CrawlerRun
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public CrawlerRun()
        {
            Data = new Dictionary<string, BaseResult>();
        }

        /// <summary>
        /// Actual result data as Dictionary with the given key for the given result.
        /// </summary>
        public Dictionary<string, BaseResult> Data { get; }

        /// <summary>
        /// Stores the sloader "run" datetime
        /// </summary>
        public DateTime RunOn { get; set; }

        /// <summary>
        /// Stores the loading or "run" time in ms
        /// </summary>
        public long RunDurationInMilliseconds { get; set; }

        /// <summary>
        /// Helper method to ensure that each key is only set once in the result Data
        /// </summary>
        /// <param name="key">Desired key</param>
        /// <param name="data">Any BaseResult for this Key</param>
        public void AddResultDataPair(string key, BaseResult data)
        {
            Trace.TraceInformation($"{nameof(AddResultDataPair)} : '{key}'");
            if (Data.ContainsKey(key) == false)
            {
                this.Data.Add(key, data);
            }
        }

        /// <summary>
        /// Serializes the object via JSON.NET
        /// </summary>
        /// <returns>Large JSON with "run" data and actual result data.</returns>
        public string ToJson(Formatting formatting = Formatting.None)
        {
            return JsonConvert.SerializeObject(this, formatting);
        }

    }
}