using System;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using FakeItEasy;
using Sloader.Config.Crawler.Feed;
using Sloader.Engine.Crawler.Feed;
using Sloader.Engine.Crawler.GitHub;
using Sloader.Result;
using Sloader.Result.Types;
using WorldDomination.Net.Http;
using Xunit;

namespace Sloader.Engine.Tests.FeedCrawlerTests
{
    public class FeedCrawlerTests
    {
        private const string samplesDirectory = "FeedCrawlerTests/Samples";
        private const string slashdotRssSample = "SlashDotRss.xml";
        private const string gitHubAtomSample = "GitHubAtom.xml";

        public async Task<FeedResult> InvokeAtomSut(int twitterCounts = 0, int facebookShares = 0, string feed = "https://github.com/robertmuehsig.atom")
        {
            string responseData = TestHelperForCurrentProject.GetTestFileContent(TestHelperForCurrentProject.GetTestFilePath(samplesDirectory, gitHubAtomSample));

            var messageResponse = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData);

            var fakeMessageHandler = new FakeHttpMessageHandler(new HttpMessageOptions() { HttpResponseMessage = messageResponse, RequestUri = feed });

            var facebokLoaderMock = A.Fake<IFacebookShareCountLoader>();
            A.CallTo(() => facebokLoaderMock.GetAsync(string.Empty)).WithAnyArguments().Returns(facebookShares);

            var sut = new FeedCrawler(fakeMessageHandler, facebokLoaderMock);

            var config = new FeedCrawlerConfig();
            config.Url = feed;

            return await sut.DoWorkAsync(config);
        }

        public async Task<FeedResult> InvokeRssWithSocialLinksEnabled(int twitterCounts = 0, int facebookShares = 0, string feed = "http://rss.slashdot.org/Slashdot/slashdot")
        {
            var sut = new FeedCrawler();
            if (feed != string.Empty)
            {
                string responseData = TestHelperForCurrentProject.GetTestFileContent(TestHelperForCurrentProject.GetTestFilePath(samplesDirectory, slashdotRssSample));

                var messageResponse = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData);

                var fakeMessageHandler = new FakeHttpMessageHandler(new HttpMessageOptions() { HttpResponseMessage = messageResponse, RequestUri = feed });
                var facebokLoaderMock = A.Fake<IFacebookShareCountLoader>();
                A.CallTo(() => facebokLoaderMock.GetAsync(string.Empty)).WithAnyArguments().Returns(facebookShares);

                sut = new FeedCrawler(fakeMessageHandler, facebokLoaderMock);
            }


            var config = new FeedCrawlerConfig();
            config.Url = feed;
            config.LoadSocialLinkCounters = true;
            config.IncludeRawContent = true;
            return await sut.DoWorkAsync(config);
        }

        [Fact]
        public async Task Crawler_Detects_Correct_Count_Of_FeedItems()
        {
            var result = await InvokeRssWithSocialLinksEnabled();

            Assert.Equal(20, result.FeedItems.Count);
        }

        [Fact]
        public async Task Crawler_Detects_Correct_PubDate()
        {
            var result = await InvokeRssWithSocialLinksEnabled();

            var test = DateTime.TryParse("Thu, 25 Dec 2014 22:22:00 GMT", out DateTime expected);

            Assert.Equal(expected, result.FeedItems.First().PublishedOn);
        }

        [Fact]
        public async Task Crawler_Detects_Correct_Count_Of_Comments()
        {
            var result = await InvokeRssWithSocialLinksEnabled();

            var staticGuidFromSampleWithThreeComments =
                "http://rss.slashdot.org/~r/Slashdot/slashdot/~3/-yqs-a_i_Qc/story01.htm";

            var specificFeedItem = result.FeedItems.Single(x => x.Href == staticGuidFromSampleWithThreeComments);

            Assert.Equal(3, specificFeedItem.CommentsCount);
        }

        [Fact]
        public async Task Crawler_Should_Embed_The_RawContent_From_The_ActualRssItem()
        {
            var result = await InvokeRssWithSocialLinksEnabled();


            var staticFeed = SyndicationFeed.Load(new XmlTextReader(TestHelperForCurrentProject.GetTestFilePath(samplesDirectory, slashdotRssSample)));
            var expectedItem = staticFeed.Items.First();

            var firstResult = result.FeedItems.Single(x => x.Title == expectedItem.Title.Text);

            Assert.True(firstResult.RawContent.Contains(expectedItem.Title.Text));
        }

        [Fact]
        public async Task Crawler_Returns_Correct_FacebookShares()
        {
            var result = await InvokeRssWithSocialLinksEnabled(0, 1337);

            foreach (var feedItem in result.FeedItems)
            {
                Assert.Equal(1337, feedItem.FacebookCount);
            }
        }

        [Fact]
        public async Task Crawler_Must_Not_Return_Null_If_Nothing_Is_Configured()
        {
            var result = await InvokeRssWithSocialLinksEnabled(0,0, string.Empty);
            Assert.NotNull(result);
        }


        [Fact]
        public async Task Crawler_Returns_Correct_Url_From_GitHubAtom()
        {
            var result = await InvokeAtomSut(0, 0);

            Assert.True(result.FeedItems.Count == 30);

            foreach (var feedItem in result.FeedItems)
            {
                Assert.True(feedItem.Href.StartsWith("https://github.com/"));
            }
        }


        [Fact]
        public async Task Crawler_Should_Not_Reach_Out_To_Facebook_If_Disabled()
        {
            string responseData = TestHelperForCurrentProject.GetTestFileContent(TestHelperForCurrentProject.GetTestFilePath(samplesDirectory, gitHubAtomSample));

            var messageResponse = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData);

            var fakeMessageHandler = new FakeHttpMessageHandler(new HttpMessageOptions() { HttpResponseMessage = messageResponse, RequestUri = "https://github.com/robertmuehsig.atom" });

            var facebokLoaderMock = A.Fake<IFacebookShareCountLoader>();

            var sut = new FeedCrawler(fakeMessageHandler, facebokLoaderMock);

            var config = new FeedCrawlerConfig();
            config.Url = "https://github.com/robertmuehsig.atom";
            config.LoadSocialLinkCounters = false;

            var result = await sut.DoWorkAsync(config);
            A.CallTo(() => facebokLoaderMock.GetAsync(string.Empty)).WithAnyArguments().MustNotHaveHappened();
        }

    }
}
