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
	        var twitterResult = new TwitterTimelineResult
	        {
		        Tweets = new List<TwitterTimelineResult.Tweet>
		        {
			        new TwitterTimelineResult.Tweet() {Id = "1"},
			        new TwitterTimelineResult.Tweet() {Id = "2"}
		        }
	        };

	        run.AddResultDataPair("test", twitterResult);

            Assert.NotNull(run.ToJson());

            Assert.Contains("test", run.ToJson());
        }
    }
}
