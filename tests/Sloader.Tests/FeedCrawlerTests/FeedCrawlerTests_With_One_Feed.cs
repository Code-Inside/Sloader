using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using FakeItEasy;
using Sloader.Bootstrapper.Crawler.Feed;
using Sloader.Types;
using WorldDomination.Net.Http;
using Xunit;

namespace Sloader.Tests.FeedCrawlerTests
{
    public class FeedCrawlerTests_With_One_Feed
    {
        private const string slashdotRssSamplePath = "FeedCrawlerTests.Samples.SlashDotRss.xml";

        public async Task<List<FeedCrawlerResult>> InvokeSut(int twitterCounts = 0, int facebookShares = 0)
        {
            var feedLoaderMock = A.Fake<IFeedLoader>();
            var staticFeed = SyndicationFeed.Load(new XmlTextReader(TestHelperForCurrentProject.GetTestFileStream(slashdotRssSamplePath)));
            A.CallTo(() => feedLoaderMock.Get("http://rss.slashdot.org/Slashdot/slashdot")).Returns(staticFeed);

            var twitterLoaderMock = A.Fake<ITwitterTweetCountLoader>();
            A.CallTo(() => twitterLoaderMock.GetAsync(string.Empty)).WithAnyArguments().Returns(twitterCounts);

            var facebokLoaderMock = A.Fake<IFacebookShareCountLoader>();
            A.CallTo(() => facebokLoaderMock.GetAsync(string.Empty)).WithAnyArguments().Returns(facebookShares);

            var sut = new FeedCrawler(feedLoaderMock, twitterLoaderMock, facebokLoaderMock);
            sut.ConfiguredFeeds = "http://rss.slashdot.org/Slashdot/slashdot";
            return await sut.DoWorkAsync();
        }

        [Fact]
        public async Task Crawler_Returns_One_FeedCrawlerResult()
        {
            var result = await InvokeSut();

            Assert.Equal(1, result.Count);
        }

        [Fact]
        public async Task Crawler_Detects_Correct_Count_Of_FeedItems()
        {
            var result = await InvokeSut();

            Assert.Equal(20, result.First().FeedItems.Count);
        }

        [Fact]
        public async Task Crawler_Returns_Correct_FacebookShares()
        {
            var result = await InvokeSut(0, 1337);

            foreach (var feedItem in result.First().FeedItems)
            {
                Assert.Equal(1337, feedItem.FacebookCount);
            }
        }

        [Fact]
        public async Task Crawler_Returns_Correct_TwitterCounts()
        {
            var result = await InvokeSut(1337, 0);

            foreach (var feedItem in result.First().FeedItems)
            {
                Assert.Equal(1337, feedItem.TweetsCount);
            }
        }
    }
}
