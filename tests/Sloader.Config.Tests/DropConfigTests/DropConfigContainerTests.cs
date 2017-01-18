using Sloader.Config.Drop;
using Xunit;

namespace Sloader.Config.Tests.DropConfigTests
{
    public class DropConfigContainerTests
    {
        [Fact]
        public void FileDrops_Is_Never_Null()
        {
            DropConfig config = new DropConfig();
            Assert.NotNull(config.FileDrops);
        }

        [Fact]
        public void GitHubDrops_Is_Never_Null()
        {
            DropConfig config = new DropConfig();
            Assert.NotNull(config.GitHubDrops);
        }

    }
}
