using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using FakeItEasy;
using Sloader.Bootstrapper.Crawler.Feed;
using Sloader.Types;
using Xunit;

namespace Sloader.Tests.FeedCrawlerTests
{
    public class FeedCrawlerTests
    {
        private const string slashdotRssSamplePath = "FeedCrawlerTests.Samples.SlashDotRss.xml";

        public List<FeedCrawlerResult> InvokeSut()
        {
            var feedLoaderMock = A.Fake<IFeedLoader>();

            var staticFeed = SyndicationFeed.Load(new XmlTextReader(TestHelperForCurrentProject.GetTestFileStream(slashdotRssSamplePath)));

            A.CallTo(() => feedLoaderMock.Get(string.Empty)).Returns(staticFeed);

            var sut = new FeedCrawler(feedLoaderMock);
            sut.ConfiguredFeeds = "http://rss.slashdot.org/Slashdot/slashdot";
            return sut.DoWorkAsync().Result;
        }

        [Fact]
        public void Crawler_Returns_One_FeedCrawlerResult()
        {
            var result = InvokeSut();

            Assert.Equal(1, result.Count);
        }

        [Fact]
        public void Crawler_Detects_Correct_Count_Of_FeedItems()
        {
            var result = InvokeSut();

            Assert.Equal(20, result.First().FeedItems.Count);
        }
    }
}
