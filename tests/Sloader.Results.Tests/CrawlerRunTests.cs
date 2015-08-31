using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sloader.Results.Tests
{
    public class CrawlerRunTests
    {
        [Fact]
        public void ToJson_Works()
        {
            CrawlerRun run = new CrawlerRun();
            var twitterResult = new TwitterTimelineCrawlerResult();
            twitterResult.Tweets = new List<TwitterTimelineCrawlerResult.Tweet>();
            twitterResult.Tweets.Add(new TwitterTimelineCrawlerResult.Tweet() { Id = "1"});
            twitterResult.Tweets.Add(new TwitterTimelineCrawlerResult.Tweet() { Id = "2" });

            run.Results.Add(twitterResult);

            Assert.NotNull(run.ToJson());
        }
    }
}
