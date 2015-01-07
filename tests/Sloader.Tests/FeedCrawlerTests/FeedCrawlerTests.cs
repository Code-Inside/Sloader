using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;
using FakeItEasy;
using Sloader.Crawler.Feed;
using Sloader.Types;
using Xunit;

namespace Sloader.Tests.FeedCrawlerTests
{
    public class FeedCrawlerTests
    {
        private const string slashdotRssSamplePath = "FeedCrawlerTests.Samples.SlashDotRss.xml";

        public async Task<FeedCrawlerResult> InvokeSut(int twitterCounts = 0, int facebookShares = 0, string feed = "http://rss.slashdot.org/Slashdot/slashdot")
        {
            var feedLoaderMock = A.Fake<IFeedLoader>();
            var staticFeed = SyndicationFeed.Load(new XmlTextReader(TestHelperForCurrentProject.GetTestFileStream(slashdotRssSamplePath)));
            A.CallTo(() => feedLoaderMock.Get("http://rss.slashdot.org/Slashdot/slashdot")).Returns(staticFeed);

            var twitterLoaderMock = A.Fake<ITwitterTweetCountLoader>();
            A.CallTo(() => twitterLoaderMock.GetAsync(string.Empty)).WithAnyArguments().Returns(twitterCounts);

            var facebokLoaderMock = A.Fake<IFacebookShareCountLoader>();
            A.CallTo(() => facebokLoaderMock.GetAsync(string.Empty)).WithAnyArguments().Returns(facebookShares);

            var sut = new FeedCrawler(feedLoaderMock, twitterLoaderMock, facebokLoaderMock);
            sut.ConfiguredFeed = feed;
            return await sut.DoWorkAsync();
        }

        [Fact]
        public async Task Crawler_Detects_Correct_Count_Of_FeedItems()
        {
            var result = await InvokeSut();

            Assert.Equal(20, result.FeedItems.Count);
        }

        [Fact]
        public async Task Crawler_Detects_Correct_Count_Of_Comments()
        {
            var result = await InvokeSut();

            var staticGuidFromSampleWithThreeComments =
                "http://slashdot.feedsportal.com/c/35028/f/647410/s/41c58897/sc/1/l/0Ltech0Bslashdot0Borg0Cstory0C140C120C250C1792590Cus0Enavy0Esells0Etop0Egun0Eaircraft0Ecarrier0Efor0Eone0Epenny0Dutm0Isource0Frss10B0Amainlinkanon0Gutm0Imedium0Ffeed/story01.htm";

            var specificFeedItem = result.FeedItems.Single(x => x.Href == staticGuidFromSampleWithThreeComments);

            Assert.Equal(3, specificFeedItem.CommentsCount);
        }

        [Fact]
        public async Task Crawler_Returns_Correct_FacebookShares()
        {
            var result = await InvokeSut(0, 1337);

            foreach (var feedItem in result.FeedItems)
            {
                Assert.Equal(1337, feedItem.FacebookCount);
            }
        }

        [Fact]
        public async Task Crawler_Must_Not_Return_Null_If_Nothing_Is_Configured()
        {
            var result = await InvokeSut(0,0, string.Empty);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Crawler_Returns_Correct_TwitterCounts()
        {
            var result = await InvokeSut(1337, 0);

            foreach (var feedItem in result.FeedItems)
            {
                Assert.Equal(1337, feedItem.TweetsCount);
            }
        }
    }
}
