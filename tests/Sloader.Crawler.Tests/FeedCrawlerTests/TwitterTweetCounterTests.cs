using System.Net;
using System.Threading.Tasks;
using Sloader.Crawler.Feed;
using WorldDomination.Net.Http;
using Xunit;

namespace Sloader.Crawler.Tests.FeedCrawlerTests
{
    public class TwitterTweetCounterTests
    {

        [Fact]
        public async Task Loader_Should_Return_0_On_HttpError()
        {
            const string responseData = "Bad Request";

            var messageResponse = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData, HttpStatusCode.BadRequest);

            HttpClientFactory.MessageHandler = new FakeHttpMessageHandler("*", messageResponse);

            var sut = new TwitterTweetCountLoader();

            Assert.Equal(0, await sut.GetAsync("http://blog.something.com"));
        }

        [Fact]
        public async Task Loader_Should_Return_TwitterCount_If_Everything_Is_Ok()
        {
            const string responseData = "{'count':1337,'url':'http://...url...'}";

            var messageResponse = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData);

            HttpClientFactory.MessageHandler = new FakeHttpMessageHandler("*", messageResponse);

            var sut = new TwitterTweetCountLoader();

            Assert.Equal(1337, await sut.GetAsync("http://blog.something.com"));
        }

        [Fact]
        public async Task Loader_Should_Return_0_Without_Any_Tweets()
        {
            const string responseData = "{'count':0,'url':'http://...url...'}";

            var messageResponse = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData);

            HttpClientFactory.MessageHandler = new FakeHttpMessageHandler("*", messageResponse);

            var sut = new TwitterTweetCountLoader();

            Assert.Equal(0, await sut.GetAsync("http://blog.something.com"));
        }
    }
}