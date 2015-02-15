using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sloader.Shared;
using Xunit;
using YamlDotNet.Serialization;

namespace Sloader.Tests.MasterCrawlerConfigTests
{
    public class TwitterTimelineConfigTests
    {
      
        [Fact]
        public void Handle_Is_Identifier()
        {
            StringBuilder fakeYaml = new StringBuilder();
            fakeYaml.AppendLine("TwitterTimelinesToCrawl:");
            fakeYaml.AppendLine("- Handle: codeinsideblog");

            var deserializer = new Deserializer();
            var result = deserializer.Deserialize<MasterCrawlerConfig>(new StringReader(fakeYaml.ToString()));
            var configValue = result.TwitterTimelinesToCrawl.First();

            Assert.Equal("codeinsideblog", configValue.ResultIdentifier);
        }
    }
}
