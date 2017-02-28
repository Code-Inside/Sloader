using Sloader.Result.Types;
using Xunit;

namespace Sloader.Result.Tests
{
    public class TwitterUserCrawlerResultTests
    {
        [Fact]
        public void Result_Has_Correct_Type()
        {
            var sut = new TwitterUserResult();
            Assert.True(sut.ResultType == KnownResultType.TwitterUser);
        }
    }
}