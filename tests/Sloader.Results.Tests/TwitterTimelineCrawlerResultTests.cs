using Xunit;

namespace Sloader.Results.Tests
{
    public class TwitterTimelineCrawlerResultTests
    {
        [Fact]
        public void Result_Has_Correct_Type()
        {
            var sut = new TwitterTimelineCrawlerResult();
            Assert.True(sut.ResultType == KnownCrawlerResultType.TwitterTimeline);
        }
    }
}
