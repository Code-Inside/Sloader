using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Sloader.Config.Crawler.Twitter;
using WorldDomination.Net.Http;
using Xunit;
using System.Linq;
using Sloader.Engine.Crawler.Twitter;
using Sloader.Result;
using Sloader.Result.Types;

namespace Sloader.Engine.Tests.TwitterUserCrawlerTests
{
    public class TwitterUserCrawlerTests
    {
        private static string GetTestFileContent()
        {
            string responseData = TestHelperForCurrentProject.GetTestFileContent("TwitterUserCrawlerTests/Sample/lookup.json");

            return responseData;
        }

        private static async Task<TwitterUserResult> InvokeSut(string oAuthToken, string handle)
        {
            string responseData = GetTestFileContent();

            var messageResponse = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData);

            var fakeMessageHandler = new FakeHttpMessageHandler(new HttpMessageOptions() { HttpResponseMessage = messageResponse });

            var sut = new TwitterUserCrawler(fakeMessageHandler);
            sut.OAuthToken = oAuthToken;

            var config = new TwitterUserCrawlerConfig();
            config.Handle = handle;

            var result = await sut.DoWorkAsync(config);
            return result;
        }

        [Fact]
        public async Task Crawler_Should_Return_One_User()
        {
            var result = await InvokeSut(Guid.NewGuid().ToString(), "Twitter");
            Assert.Equal(1, result.Users.Count);
        }

        [Fact]
        public async Task Crawler_Should_Embed_The_RawContent_From_The_ActualUserItem()
        {
            var result = await InvokeSut(Guid.NewGuid().ToString(), "Twitter");

            var firstResult = result.Users.First();

            var testContent = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(GetTestFileContent());

            var firstTestContent = testContent.First.ToString();

            Assert.Equal(firstTestContent, firstResult.RawContent);
        }


        [Fact]
        public async Task Crawler_Must_Not_Return_Null_If_Nothing_Is_Configured()
        {
            var result = await InvokeSut(string.Empty, string.Empty);
            Assert.NotNull(result);
        }
    }
}