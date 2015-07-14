using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Sloader.Crawler.Config.Tests.SloaderConfigTests
{
    public class TwitterUserConfigTests
    {

        [Fact]
        public void Handle_Is_Identifier()
        {
            StringBuilder fakeYaml = new StringBuilder();
            fakeYaml.AppendLine("Crawler:");
            fakeYaml.AppendLine("  TwitterUsersToCrawl:");
            fakeYaml.AppendLine("  - Handle: codeinsideblog");

            var deserializer = Constants.SloaderYamlDeserializer;
            var result = deserializer.Deserialize<SloaderConfig>(new StringReader(fakeYaml.ToString()));
            var configValue = result.Crawler.TwitterUsersToCrawl.First();

            Assert.Equal("codeinsideblog", configValue.ResultIdentifier);
        }
    }
}