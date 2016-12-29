using System.Collections.Generic;
using Sloader.Result.Types;
using Xunit;

namespace Sloader.Result.Tests
{
    public class CrawlerRunTests
    {
        [Fact]
        public void ToJson_Works()
        {
            CrawlerRun run = new CrawlerRun();
            var twitterResult = new TwitterTimelineResult();
            twitterResult.Tweets = new List<TwitterTimelineResult.Tweet>();
            twitterResult.Tweets.Add(new TwitterTimelineResult.Tweet() { Id = "1"});
            twitterResult.Tweets.Add(new TwitterTimelineResult.Tweet() { Id = "2" });

            run.Data.Add("test", twitterResult);

            Assert.NotNull(run.ToJson());
        }
    }
}
