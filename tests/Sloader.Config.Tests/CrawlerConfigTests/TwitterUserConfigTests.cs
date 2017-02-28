using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Sloader.Config.Tests.CrawlerConfigTests
{
    public class TwitterUserConfigTests
    {

        [Fact]
        public void Key_Is_Identifier()
        {
            StringBuilder fakeYaml = new StringBuilder();
            fakeYaml.AppendLine("Crawler:");
            fakeYaml.AppendLine("  TwitterUsersToCrawl:");
            fakeYaml.AppendLine("  - Handle: codeinsideblog");
            fakeYaml.AppendLine("    Key: CIB");

            var deserializer = Constants.SloaderYamlDeserializer;
            var result = deserializer.Deserialize<SloaderConfig>(new StringReader(fakeYaml.ToString()));
            var configValue = result.Crawler.TwitterUsersToCrawl.First();

            Assert.Equal("CIB", configValue.Key);
        }
    }


}