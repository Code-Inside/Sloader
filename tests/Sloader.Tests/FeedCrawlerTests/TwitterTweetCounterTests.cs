using System.Net;
using System.Threading.Tasks;
using Sloader.Bootstrapper.Crawler.Feed;
using WorldDomination.Net.Http;
using Xunit;

namespace Sloader.Tests.FeedCrawlerTests
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
    }
}