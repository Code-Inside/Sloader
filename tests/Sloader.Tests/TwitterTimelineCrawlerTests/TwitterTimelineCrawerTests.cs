using System;
using System.Threading.Tasks;
using Sloader.Crawler.Twitter;
using Sloader.Types;
using WorldDomination.Net.Http;
using Xunit;

namespace Sloader.Tests.TwitterTimelineCrawlerTests
{
    public class TwitterTimelineCrawerTests
    {
        private static async Task<TwitterTimelineCrawlerResult> InvokeSut(string oAuthToken, string handle, string resultIdentifier)
        {
            string responseData =
                TestHelperForCurrentProject.GetTestFileContent("TwitterTimelineCrawlerTests.Sample.user_timeline.json");

            var messageResponse = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData);

            HttpClientFactory.MessageHandler = new FakeHttpMessageHandler("*", messageResponse);

            var sut = new TwitterTimelineCrawler();
            sut.Handle = handle;
            sut.OAuthToken = oAuthToken;
            var result = await sut.DoWorkAsync(resultIdentifier);
            return result;
        }

        [Fact]
        public async Task Crawler_Should_Return_Correct_Number_Of_Tweets_In_Timeline()
        {
            var result = await InvokeSut(Guid.NewGuid().ToString(), "test", "resultIdenfier");
            Assert.Equal(5, result.Tweets.Count);
        }


        [Fact]
        public async Task Crawler_Must_Not_Return_Null_If_Nothing_Is_Configured()
        {
            var result = await InvokeSut(string.Empty, string.Empty, "resultIdenfier");
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Crawler_Returns_Result_With_Correct_Identifier()
        {
            var result = await InvokeSut(Guid.NewGuid().ToString(), "test", "resultIdenfier1234");
            Assert.Equal("resultIdenfier1234", result.ResultIdentifier);
        }
    }
}
