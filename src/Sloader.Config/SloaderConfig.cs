using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sloader.Config
{
    public class SloaderConfig
    {
        public SloaderConfig()
        {
            Secrets = new SloaderSecrets();
        }

        public async static Task<SloaderConfig> Load(string yamlLocation, Dictionary<string, string> secrets)
        {
            return await SloaderConfigLoader.GetAsync(yamlLocation, secrets);
        }

        public SloaderSecrets Secrets { get; set; }

        public static Task Load(object p1, object p2)
        {
            throw new NotImplementedException();
        }

        public CrawlerConfig Crawler { get; set; }

        public DropConfig Drop { get; set; }

    }
}