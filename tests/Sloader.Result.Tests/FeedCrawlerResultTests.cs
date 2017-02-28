using Sloader.Result.Types;
using Xunit;

namespace Sloader.Result.Tests
{
    public class FeedCrawlerResultTests
    {
        [Fact]
        public void Result_Has_Correct_Type()
        {
            var sut = new FeedResult();
            Assert.True(sut.ResultType == KnownResultType.Feed);
        }
    }
}
