﻿using System.IO;
using System.Threading.Tasks;
using Sloader.Shared;
using Xunit;
using YamlDotNet.Serialization;

namespace Sloader.Tests.MasterCrawlerConfigTests
{
    public class MasterCrawlerConfigTests
    {
        [Fact]
        public async Task Deserialize_Of_Testdata_To_MasterCrawlerConfig_Works()
        {
            string yaml = TestHelperForCurrentProject.GetTestFileContent("MasterCrawlerConfigTests.Sample.yaml");

            var deserializer = new Deserializer();
            var result = deserializer.Deserialize<MasterCrawlerConfig>(new StringReader(yaml));

            Assert.Equal(3, result.FeedsToCrawl.Count);
            Assert.Equal(3, result.TwitterTimelinesToCrawl.Count);
        }


    }
}