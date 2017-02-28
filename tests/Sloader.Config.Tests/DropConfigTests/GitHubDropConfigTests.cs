using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Sloader.Config.Tests.DropConfigTests
{
    public class GitHubDropConfigTests
    {
        public SloaderConfig InvokeSut()
        {
            StringBuilder fakeYaml = new StringBuilder();
            fakeYaml.AppendLine("Drop:");
            fakeYaml.AppendLine("  GitHubDrops:");
            fakeYaml.AppendLine("  - Owner: \"Code-Inside\"");
            fakeYaml.AppendLine("    Repo: \"Hub\"");
            fakeYaml.AppendLine("    Branch: \"gh-pages\"");
            fakeYaml.AppendLine("    FilePath: \"_data/test2.json\"");
            fakeYaml.AppendLine("  - Owner: \"Code-InsideX\"");
            fakeYaml.AppendLine("    Repo: \"HubX\"");
            fakeYaml.AppendLine("    Branch: \"gh-pagesX\"");
            fakeYaml.AppendLine("    FilePath: \"_data/test1.jsonX\"");

            var deserializer = Constants.SloaderYamlDeserializer;
            var result = deserializer.Deserialize<SloaderConfig>(new StringReader(fakeYaml.ToString()));

            return result;
        }

        [Fact]
        public void Can_Detect_Multiple_GitHubDropss()
        {
            Assert.True(InvokeSut().Drop.GitHubDrops.Count == 2);
        }

        [Fact]
        public void Can_Parse_FilePath()
        {
            Assert.True(InvokeSut().Drop.GitHubDrops.FirstOrDefault().FilePath == "_data/test2.json");
        }

        [Fact]
        public void Can_Parse_Repo()
        {
            Assert.True(InvokeSut().Drop.GitHubDrops.FirstOrDefault().Repo == "Hub");
        }

        [Fact]
        public void Can_Parse_Owner()
        {
            Assert.True(InvokeSut().Drop.GitHubDrops.FirstOrDefault().Owner == "Code-Inside");
        }

        [Fact]
        public void Can_Parse_Branch()
        {
            Assert.True(InvokeSut().Drop.GitHubDrops.FirstOrDefault().Branch == "gh-pages");
        }
    }
}