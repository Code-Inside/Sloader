using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sloader.Config.Crawler;
using Xunit;

namespace Sloader.Config.Tests.CrawlerConfigTests
{
    public class CrawlerConfigContainerTests
    {
        [Fact]
        public void FeedsToCrawl_Is_Never_Null()
        {
            CrawlerConfig config = new CrawlerConfig();
            Assert.NotNull(config.FeedsToCrawl);
        }

        [Fact]
        public void TwitterTimelinesToCrawl_Is_Never_Null()
        {
            CrawlerConfig config = new CrawlerConfig();
            Assert.NotNull(config.TwitterTimelinesToCrawl);
        }

        [Fact]
        public void TwitterUsersToCrawl_Is_Never_Null()
        {
            CrawlerConfig config = new CrawlerConfig();
            Assert.NotNull(config.TwitterUsersToCrawl);
        }
    }
}
