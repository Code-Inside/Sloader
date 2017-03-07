using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Sloader.Config.Tests.DropConfigTests
{
    public class FileDropConfigTests
    {
        public SloaderConfig InvokeSut()
        {
            StringBuilder fakeYaml = new StringBuilder();
            fakeYaml.AppendLine("Drop:");
            fakeYaml.AppendLine("  FileDrops:");
            fakeYaml.AppendLine("  - FilePath: test1.json");
            fakeYaml.AppendLine("  - FilePath: test2.json");

            var deserializer = Constants.SloaderYamlDeserializer;
            var result = deserializer.Deserialize<SloaderConfig>(new StringReader(fakeYaml.ToString()));

            return result;
        }

        [Fact]
        public void Can_Detect_Multiple_FilePaths()
        {
            Assert.True(InvokeSut().Drop.FileDrops.Count == 2);
        }

        [Fact]
        public void Can_Parse_FilePath()
        {
            Assert.True(InvokeSut().Drop.FileDrops.FirstOrDefault().FilePath == "test1.json");
        }
    }
}
