using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using FakeItEasy;
using Sloader.Config.Crawler.Feed;
using Sloader.Engine.Crawler.Feed;
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
        private const string nugetBlogAtomSample = "NuGetBlogAtom.xml";
        private const string msWebDevRssSample = "MSWebDevRssWithCategories.xml";


        public async Task<FeedResult> InvokeAtomSut(int twitterCounts = 0, int facebookShares = 0, string feed = "https://github.com/robertmuehsig.atom", int truncateAt = 0)
        {
            var atomXmlFile = gitHubAtomSample;

            if (feed == "https://blog.nuget.org/feeds/atom.xml")
            {
                atomXmlFile = nugetBlogAtomSample;
            }

            string responseData = TestHelperForCurrentProject.GetTestFileContent(TestHelperForCurrentProject.GetTestFilePath(samplesDirectory, atomXmlFile));

            var messageResponse = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData);

            var fakeMessageHandler = new FakeHttpMessageHandler(new HttpMessageOptions() { HttpResponseMessage = messageResponse, RequestUri = new Uri(feed) });

            var facebokLoaderMock = A.Fake<IFacebookShareCountLoader>();
            A.CallTo(() => facebokLoaderMock.GetAsync(string.Empty)).WithAnyArguments().Returns(facebookShares);

            var sut = new FeedCrawler(fakeMessageHandler, facebokLoaderMock);

            var config = new FeedCrawlerConfig
            {
                Url = feed,
                SummaryTruncateAt = truncateAt
            };
            return await sut.DoWorkAsync(config);
        }

        public async Task<FeedResult> InvokeRssWithSocialLinksEnabled(int twitterCounts = 0, int facebookShares = 0, string feed = "http://rss.slashdot.org/Slashdot/slashdot", int truncateAt = 0)
        {
            var sut = new FeedCrawler();
            if (feed != string.Empty)
            {
                string responseData = TestHelperForCurrentProject.GetTestFileContent(TestHelperForCurrentProject.GetTestFilePath(samplesDirectory, slashdotRssSample));

                var messageResponse = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData);

                var fakeMessageHandler = new FakeHttpMessageHandler(new HttpMessageOptions() { HttpResponseMessage = messageResponse, RequestUri = new Uri(feed) });
                var facebokLoaderMock = A.Fake<IFacebookShareCountLoader>();
                A.CallTo(() => facebokLoaderMock.GetAsync(string.Empty)).WithAnyArguments().Returns(facebookShares);

                sut = new FeedCrawler(fakeMessageHandler, facebokLoaderMock);
            }


            var config = new FeedCrawlerConfig
            {
                Url = feed,
                LoadSocialLinkCounters = true,
                IncludeRawContent = true,
                SummaryTruncateAt = truncateAt
            };

            return await sut.DoWorkAsync(config);
        }


        public async Task<FeedResult> InvokeRssWithCategories(List<string> categories, string feed = "https://blogs.msdn.microsoft.com/webdev/feed/", int truncateAt = 0)
        {
            var sut = new FeedCrawler();
            if (feed != string.Empty)
            {
                string responseData = TestHelperForCurrentProject.GetTestFileContent(TestHelperForCurrentProject.GetTestFilePath(samplesDirectory, msWebDevRssSample));

                var messageResponse = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData);

                var fakeMessageHandler = new FakeHttpMessageHandler(new HttpMessageOptions() { HttpResponseMessage = messageResponse, RequestUri = new Uri(feed) });
                var facebokLoaderMock = A.Fake<IFacebookShareCountLoader>();

                sut = new FeedCrawler(fakeMessageHandler, facebokLoaderMock);
            }


            var config = new FeedCrawlerConfig
            {
                Url = feed,
                LoadSocialLinkCounters = false,
                IncludeRawContent = true,
                SummaryTruncateAt = truncateAt,
                FilterByCategories = categories
            };

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
        public async Task Crawler_Returns_Can_Filter_RSSEntries_ByCategories()
        {
            var result = await InvokeRssWithCategories(new List<string> { "Blazor" });

            Assert.True(result.FeedItems.Count == 1);
            Assert.True(result.FeedItems[0].Href == "https://blogs.msdn.microsoft.com/webdev/2018/11/15/blazor-0-7-0-experimental-release-now-available/");
        }

        [Fact]
        public async Task Crawler_Returns_Can_Filter_RSSEntries_ByCategories_Large_CategoryList()
        {
            var result = await InvokeRssWithCategories(new List<string> { "Visual Studio", "Azure" });

            Assert.True(result.FeedItems.Count == 2);
            Assert.Contains(result.FeedItems, x => x.Href == "https://blogs.msdn.microsoft.com/webdev/2018/11/09/when-should-you-right-click-publish/");
            Assert.Contains(result.FeedItems, x => x.Href == "https://blogs.msdn.microsoft.com/webdev/2018/10/04/use-hybrid-connections-to-incrementally-migrate-applications-to-the-cloud/");
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
                Assert.StartsWith("https://github.com/", feedItem.Href);
            }
        }

        [Fact]
        public async Task Crawler_Returns_Correct_Date_From_Atom_With_Only_Updated_Entry()
        {
            var result = await InvokeAtomSut(0, 0, "https://blog.nuget.org/feeds/atom.xml");


            foreach (var feedItem in result.FeedItems)
            {
                Assert.True(feedItem.PublishedOn != DateTime.MinValue);
            }
        }

        [Fact]
        public async Task Crawler_With_Truncation_Should_Return_Only_Desired_CharCount_For_Atom()
        {
            var result = await InvokeAtomSut(0, 0, "https://blog.nuget.org/feeds/atom.xml", 100);


            foreach (var feedItem in result.FeedItems)
            {
                // including 3 ...
                Assert.True(feedItem.Summary.Length <= 103);
                Assert.DoesNotContain("<", feedItem.Summary);
            }
        }

        [Fact]
        public async Task Crawler_With_Truncation_Should_Return_Only_Desired_CharCount_For_RSS()
        {
            var result = await InvokeRssWithSocialLinksEnabled(0, 0, "http://rss.slashdot.org/Slashdot/slashdot", 20);


            foreach (var feedItem in result.FeedItems)
            {
                // including 3 ...
                Assert.True(feedItem.Summary.Length <= 23);
                Assert.DoesNotContain("<", feedItem.Summary);
            }
        }


        [Fact]
        public async Task Crawler_Should_Not_Reach_Out_To_Facebook_If_Disabled()
        {
            string responseData = TestHelperForCurrentProject.GetTestFileContent(TestHelperForCurrentProject.GetTestFilePath(samplesDirectory, gitHubAtomSample));

            var messageResponse = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData);

            var fakeMessageHandler = new FakeHttpMessageHandler(new HttpMessageOptions() { HttpResponseMessage = messageResponse, RequestUri = new Uri("https://github.com/robertmuehsig.atom") });

            var facebokLoaderMock = A.Fake<IFacebookShareCountLoader>();

            var sut = new FeedCrawler(fakeMessageHandler, facebokLoaderMock);

            var config = new FeedCrawlerConfig
            {
                Url = "https://github.com/robertmuehsig.atom",
                LoadSocialLinkCounters = false
            };

            var result = await sut.DoWorkAsync(config);
            A.CallTo(() => facebokLoaderMock.GetAsync(string.Empty)).WithAnyArguments().MustNotHaveHappened();
        }

    }
}
