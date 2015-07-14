using Xunit;

namespace Sloader.Results.Tests
{
    public class TwitterUserCrawlerResultTests
    {
        [Fact]
        public void Result_Has_Correct_Type()
        {
            var sut = new TwitterUserCrawlerResult();
            Assert.True(sut.ResultType == KnownCrawlerResultType.TwitterUser);
        }
    }
}