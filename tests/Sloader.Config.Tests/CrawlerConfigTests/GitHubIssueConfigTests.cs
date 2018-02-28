using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Sloader.Config.Tests.CrawlerConfigTests
{
    public class GitHubIssueConfigTests
    {

        [Fact]
        public void Can_Convert()
        {
            StringBuilder fakeYaml = new StringBuilder();
            fakeYaml.AppendLine("Crawler:");
            fakeYaml.AppendLine("  GitHubIssuesToCrawl:");
            fakeYaml.AppendLine("   - Repository: code-inside/sloader");
            fakeYaml.AppendLine("     Key: Test");

            var deserializer = Constants.SloaderYamlDeserializer;
            var result = deserializer.Deserialize<SloaderConfig>(new StringReader(fakeYaml.ToString()));
            var configValue = result.Crawler.GitHubIssuesToCrawl.First();

            Assert.Equal("Test", configValue.Key);
            Assert.Equal("code-inside/sloader", configValue.Repository);
        }

    }
}