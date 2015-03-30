using System;
using System.IO;
using System.Threading.Tasks;
using Sloader.Crawler.Config.Twitter;
using Sloader.Crawler.Twitter;
using Sloader.Results;
using WorldDomination.Net.Http;
using Xunit;

namespace Sloader.Tests.TwitterTimelineCrawlerTests
{
    public class TwitterTimelineCrawerTests
    {
        private static async Task<TwitterTimelineCrawlerResult> InvokeSut(string oAuthToken, string handle)
        {
            string responseData = TestHelperForCurrentProject.GetTestFileContent("TwitterTimelineCrawlerTests/Sample/user_timeline.json");

            var messageResponse = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData);

            HttpClientFactory.MessageHandler = new FakeHttpMessageHandler("*", messageResponse);

            var sut = new TwitterTimelineCrawler();
            sut.OAuthToken = oAuthToken;

            var config = new TwitterTimelineCrawlerConfig();
            config.Handle = handle;

            var result = await sut.DoWorkAsync(config);
            return result;
        }

        [Fact]
        public async Task Crawler_Should_Return_Correct_Number_Of_Tweets_In_Timeline()
        {
            var result = await InvokeSut(Guid.NewGuid().ToString(), "test");
            Assert.Equal(5, result.Tweets.Count);
        }


        [Fact]
        public async Task Crawler_Must_Not_Return_Null_If_Nothing_Is_Configured()
        {
            var result = await InvokeSut(string.Empty, string.Empty);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Crawler_Returns_Result_With_Correct_Identifier()
        {
            var result = await InvokeSut(Guid.NewGuid().ToString(), "test");
            Assert.Equal("test", result.ResultIdentifier);
        }
    }

    public class TwitterTimelineCrawlerResultTests
    {
        [Fact]
        public void Result_Has_Correct_Type()
        {
            var sut = new TwitterTimelineCrawlerResult();
            Assert.True(sut.ResultType == KnownCrawlerResultType.TwitterTimeline);
        }
    }
}
