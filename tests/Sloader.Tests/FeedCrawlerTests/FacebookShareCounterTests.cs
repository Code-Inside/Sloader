using System.Net;
using System.Threading.Tasks;
using Sloader.Bootstrapper.Crawler.Feed;
using WorldDomination.Net.Http;
using Xunit;

namespace Sloader.Tests.FeedCrawlerTests
{
    public class FacebookShareCounterTests
    {

        [Fact]
        public async Task Loader_Should_Return_0_On_HttpError()
        {
            const string responseData = "Bad Request";

            var messageResponse = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData, HttpStatusCode.BadRequest);

            HttpClientFactory.MessageHandler = new FakeHttpMessageHandler("*", messageResponse);

            var sut = new FacebookShareCountLoader();

            Assert.Equal(0, await sut.GetAsync("http://blog.something.com"));
        }

        [Fact]
        public async Task Loader_Should_Return_ShareCount_If_Everything_Is_Ok()
        {
            const string responseData = "{'id':'http://...url...','shares':1337}";

            var messageResponse = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData);

            HttpClientFactory.MessageHandler = new FakeHttpMessageHandler("*", messageResponse);

            var sut = new FacebookShareCountLoader();

            Assert.Equal(1337, await sut.GetAsync("http://blog.something.com"));
        }

        [Fact]
        public async Task Loader_Should_Return_0_Without_Any_Shares()
        {
            const string responseData = "{'id':'http://...url...'}";

            var messageResponse = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData);

            HttpClientFactory.MessageHandler = new FakeHttpMessageHandler("*", messageResponse);

            var sut = new FacebookShareCountLoader();

            Assert.Equal(0, await sut.GetAsync("http://blog.something.com"));
        }
    }
}