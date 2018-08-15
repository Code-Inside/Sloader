using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Sloader.Config.Crawler.Twitter;
using Sloader.Engine.Crawler.Twitter;
using Sloader.Result;
using Sloader.Result.Types;
using WorldDomination.Net.Http;
using Xunit;

namespace Sloader.Engine.Tests.TwitterTimelineCrawlerTests
{
    public class TwitterTimelineCrawlerTests
    {
        private static string GetTestFileContent()
        {
            string responseData = TestHelperForCurrentProject.GetTestFileContent("TwitterTimelineCrawlerTests/Sample/user_timeline.json");

            return responseData;
        }

        private static async Task<TwitterTimelineResult> InvokeSut(string oAuthToken, string handle, bool includeRaw = false)
        {
            string responseData = GetTestFileContent();

            var messageResponse = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData);

            var fakeMessageHandler = new FakeHttpMessageHandler(new HttpMessageOptions() { HttpResponseMessage = messageResponse });

            var sut = new TwitterTimelineCrawler(fakeMessageHandler);
            sut.OAuthToken = oAuthToken;

            var config = new TwitterTimelineCrawlerConfig();
            config.Handle = handle;
            config.IncludeRawContent = includeRaw;

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
        public async Task Crawler_Should_Embed_The_RawContent_From_The_ActualTimeLineItem_WhenConfigured()
        {
            var result = await InvokeSut(Guid.NewGuid().ToString(), "test", true);

            var firstResult = result.Tweets.First();

            var testContent = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(GetTestFileContent());

            var firstTestContent = testContent.First.ToString();

            Assert.Equal(firstTestContent, firstResult.RawContent);
        }

        [Fact]
        public async Task Crawler_Shouldnt_Embed_The_RawContent_From_The_ActualTimeLineItem()
        {
            var result = await InvokeSut(Guid.NewGuid().ToString(), "test");

            var firstResult = result.Tweets.First();

            var testContent = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(GetTestFileContent());

            var firstTestContent = testContent.First.ToString();

            Assert.Null(firstResult.RawContent);
        }


        [Fact]
        public async Task Crawler_Must_Not_Return_Null_If_Nothing_Is_Configured()
        {
            var result = await InvokeSut(string.Empty, string.Empty);
            Assert.NotNull(result);
        }
    }
}