using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;
using FakeItEasy;
using Sloader.Crawler.Config.Feed;
using Sloader.Crawler.Feed;
using Sloader.Results;
using Xunit;

namespace Sloader.Tests.FeedCrawlerTests
{
    public class FeedCrawlerTests
    {
        private const string slashdotRssSamplePath = "FeedCrawlerTests.Samples.SlashDotRss.xml";
        private const string gitHubAtomSamplePath = "FeedCrawlerTests.Samples.GitHubAtom.xml";

        public async Task<FeedCrawlerResult> InvokeGitHubSut(int twitterCounts = 0, int facebookShares = 0, string feed = "https://github.com/robertmuehsig.atom")
        {
            var feedLoaderMock = A.Fake<ISyndicationFeedAbstraction>();
            var staticFeed = SyndicationFeed.Load(new XmlTextReader(TestHelperForCurrentProject.GetTestFileStream(gitHubAtomSamplePath)));
            A.CallTo(() => feedLoaderMock.Get("https://github.com/robertmuehsig.atom")).Returns(staticFeed);

            var twitterLoaderMock = A.Fake<ITwitterTweetCountLoader>();
            A.CallTo(() => twitterLoaderMock.GetAsync(string.Empty)).WithAnyArguments().Returns(twitterCounts);

            var facebokLoaderMock = A.Fake<IFacebookShareCountLoader>();
            A.CallTo(() => facebokLoaderMock.GetAsync(string.Empty)).WithAnyArguments().Returns(facebookShares);

            var sut = new FeedCrawler(feedLoaderMock, twitterLoaderMock, facebokLoaderMock);

            var config = new FeedCrawlerConfig();
            config.Url = feed;

            return await sut.DoWorkAsync(config);
        }


        public async Task<FeedCrawlerResult> InvokeSlashdotSut(int twitterCounts = 0, int facebookShares = 0, string feed = "http://rss.slashdot.org/Slashdot/slashdot")
        {
            var feedLoaderMock = A.Fake<ISyndicationFeedAbstraction>();
            var staticFeed = SyndicationFeed.Load(new XmlTextReader(TestHelperForCurrentProject.GetTestFileStream(slashdotRssSamplePath)));
            A.CallTo(() => feedLoaderMock.Get("http://rss.slashdot.org/Slashdot/slashdot")).Returns(staticFeed);

            var twitterLoaderMock = A.Fake<ITwitterTweetCountLoader>();
            A.CallTo(() => twitterLoaderMock.GetAsync(string.Empty)).WithAnyArguments().Returns(twitterCounts);

            var facebokLoaderMock = A.Fake<IFacebookShareCountLoader>();
            A.CallTo(() => facebokLoaderMock.GetAsync(string.Empty)).WithAnyArguments().Returns(facebookShares);

            var sut = new FeedCrawler(feedLoaderMock, twitterLoaderMock, facebokLoaderMock);

            var config = new FeedCrawlerConfig();
            config.Url = feed;

            return await sut.DoWorkAsync(config);
        }

        [Fact]
        public async Task Crawler_Detects_Correct_Count_Of_FeedItems()
        {
            var result = await InvokeSlashdotSut();

            Assert.Equal(20, result.FeedItems.Count);
        }

        [Fact]
        public async Task Crawler_Detects_Correct_Count_Of_Comments()
        {
            var result = await InvokeSlashdotSut();

            var staticGuidFromSampleWithThreeComments =
                "http://rss.slashdot.org/~r/Slashdot/slashdot/~3/-yqs-a_i_Qc/story01.htm";

            var specificFeedItem = result.FeedItems.Single(x => x.Href == staticGuidFromSampleWithThreeComments);

            Assert.Equal(3, specificFeedItem.CommentsCount);
        }

        [Fact]
        public async Task Crawler_Returns_Correct_FacebookShares()
        {
            var result = await InvokeSlashdotSut(0, 1337);

            foreach (var feedItem in result.FeedItems)
            {
                Assert.Equal(1337, feedItem.FacebookCount);
            }
        }

        [Fact]
        public async Task Crawler_Must_Not_Return_Null_If_Nothing_Is_Configured()
        {
            var result = await InvokeSlashdotSut(0,0, string.Empty);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Crawler_Returns_Correct_TwitterCounts()
        {
            var result = await InvokeSlashdotSut(1337, 0);

            foreach (var feedItem in result.FeedItems)
            {
                Assert.Equal(1337, feedItem.TweetsCount);
            }
        }


        [Fact]
        public async Task Crawler_Returns_Correct_Url_From_GitHubAtom()
        {
            var result = await InvokeGitHubSut(0, 0);

            foreach (var feedItem in result.FeedItems)
            {
                Assert.True(feedItem.Href.StartsWith("https://github.com/"));
            }
        }


        [Fact]
        public async Task Crawler_Returns_Result_With_Correct_Identifier()
        {
            var result = await InvokeGitHubSut(0, 0);
            Assert.Equal("https://github.com/robertmuehsig.atom", result.ResultIdentifier);
        }

    }
}
