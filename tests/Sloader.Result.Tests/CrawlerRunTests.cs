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
	        var feedResult = new FeedResult
	        {
		        FeedItems = new List<FeedResult.FeedItem>
		        {
			        new FeedResult.FeedItem { Title = "1"},
                    new FeedResult.FeedItem { Title = "2"},
		        }
	        };

	        run.AddResultDataPair("test", feedResult);

            Assert.NotNull(run.ToJson());

            Assert.Contains("test", run.ToJson());
        }
    }
}
