using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Sloader.Config.Tests.CrawlerConfigTests
{
    public class GitHubEventConfigTests
    {

        [Fact]
        public void Can_Convert()
        {
            StringBuilder fakeYaml = new StringBuilder();
            fakeYaml.AppendLine("Crawler:");
            fakeYaml.AppendLine("  GitHubEventsToCrawl:");
            fakeYaml.AppendLine("  - Organization: codeinside");
            fakeYaml.AppendLine("    User: robertmuehsig");
            fakeYaml.AppendLine("    Repository: code-inside/sloader");
            fakeYaml.AppendLine("    Key: Test");

            var deserializer = Constants.SloaderYamlDeserializer;
            var result = deserializer.Deserialize<SloaderConfig>(new StringReader(fakeYaml.ToString()));
            var configValue = result.Crawler.GitHubEventsToCrawl.First();

            Assert.Equal("Test", configValue.Key);
            Assert.Equal("codeinside", configValue.Organization);
            Assert.Equal("robertmuehsig", configValue.User);
            Assert.Equal("code-inside/sloader", configValue.Repository);

        }
    }
}