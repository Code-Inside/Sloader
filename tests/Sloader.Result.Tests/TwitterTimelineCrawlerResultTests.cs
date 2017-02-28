using Sloader.Result.Types;
using Xunit;

namespace Sloader.Result.Tests
{
    public class TwitterTimelineCrawlerResultTests
    {
        [Fact]
        public void Result_Has_Correct_Type()
        {
            var sut = new TwitterTimelineResult();
            Assert.True(sut.ResultType == KnownResultType.TwitterTimeline);
        }
    }
}
