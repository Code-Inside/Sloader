using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sloader.Crawler.Config;
using Xunit;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Sloader.Tests.MasterCrawlerConfigTests
{
    public class FeedCrawlerConfigTests
    {
        [Fact]
        public void Default_For_LoadSocialLinkCounters_Is_True()
        {
            StringBuilder fakeYaml = new StringBuilder();
            fakeYaml.AppendLine("FeedsToCrawl:");
            fakeYaml.AppendLine("- Url: http://blogin.codeinside.eu/feed");

            var deserializer = new Deserializer();
            var result = deserializer.Deserialize<MasterCrawlerConfig>(new StringReader(fakeYaml.ToString()));
            var configValue = result.FeedsToCrawl.First();

            Assert.True(configValue.LoadSocialLinkCounters);
        }

        [Fact]
        public void Url_Is_Identifier()
        {
            StringBuilder fakeYaml = new StringBuilder();
            fakeYaml.AppendLine("FeedsToCrawl:");
            fakeYaml.AppendLine("- Url: http://blogin.codeinside.eu/feed");

            var deserializer = new Deserializer();
            var result = deserializer.Deserialize<MasterCrawlerConfig>(new StringReader(fakeYaml.ToString()));
            var configValue = result.FeedsToCrawl.First();

            Assert.Equal("http://blogin.codeinside.eu/feed", configValue.ResultIdentifier);
        }
    }
}
