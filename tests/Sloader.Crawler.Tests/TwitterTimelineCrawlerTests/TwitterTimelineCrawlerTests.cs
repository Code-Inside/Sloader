using System;
using System.Threading.Tasks;
using Sloader.Crawler.Config.Twitter;
using Sloader.Crawler.Twitter;
using Sloader.Results;
using WorldDomination.Net.Http;
using Xunit;

namespace Sloader.Crawler.Tests.TwitterTimelineCrawlerTests
{
    public class TwitterTimelineCrawlerTests
    {
        private static async Task<TwitterTimelineCrawlerResult> InvokeSut(string oAuthToken, string handle)
        {
            string responseData = TestHelperForCurrentProject.GetTestFileContent("TwitterTimelineCrawlerTests/Sample/user_timeline.json");

            var messageResponse = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData);

            var fakeMessageHandler = new FakeHttpMessageHandler("*", messageResponse);

            var sut = new TwitterTimelineCrawler(fakeMessageHandler);
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
}