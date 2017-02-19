using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Sloader.Result.Types;
using WorldDomination.Net.Http;
using Xunit;
using Sloader.Engine.Crawler.GitHub;
using Sloader.Config.Crawler.GitHub;

namespace Sloader.Engine.Tests.GitHubEventCrawlerTests
{
    public class GitHubEventCrawlerTests
    {
        private static string GetTestFileContentFor(string topic)
        {
            string responseData = TestHelperForCurrentProject.GetTestFileContent($"GitHubEventCrawlerTests/Sample/{topic}.json");

            return responseData;
        }

        private static async Task<GitHubEventResult> InvokeSutForOrgs(string org)
        {
            string responseData = GetTestFileContentFor("org");

            var messageResponse = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData);

            var fakeMessageHandler = new FakeHttpMessageHandler(new HttpMessageOptions() { HttpResponseMessage = messageResponse, RequestUri = $"https://api.github.com/orgs/{org}/events" });

            var sut = new GitHubEventCrawler(fakeMessageHandler);

            var config = new GitHubEventCrawlerConfig();
            config.Organization = org;

            var result = await sut.DoWorkAsync(config);
            return result;
        }

        private static async Task<GitHubEventResult> InvokeSutForUsers(string user)
        {
            string responseData = GetTestFileContentFor("user");

            var messageResponse = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData);

            var fakeMessageHandler = new FakeHttpMessageHandler(new HttpMessageOptions() { HttpResponseMessage = messageResponse, RequestUri = $"https://api.github.com/users/{user}/events" });

            var sut = new GitHubEventCrawler(fakeMessageHandler);

            var config = new GitHubEventCrawlerConfig();
            config.User = user;

            var result = await sut.DoWorkAsync(config);
            return result;
        }

        private static async Task<GitHubEventResult> InvokeSutForRepos(string repo)
        {
            string responseData = GetTestFileContentFor("repo");

            var messageResponse = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData);

            var fakeMessageHandler = new FakeHttpMessageHandler(new HttpMessageOptions() { HttpResponseMessage = messageResponse, RequestUri = $"https://api.github.com/repos/{repo}/events" });

            var sut = new GitHubEventCrawler(fakeMessageHandler);

            var config = new GitHubEventCrawlerConfig();
            config.Repository = repo;

            var result = await sut.DoWorkAsync(config);
            return result;
        }

        [Fact]
        public async Task Crawler_For_Orgs_Should_Return_Correct_Number_Of_Events()
        {
            var result = await InvokeSutForOrgs("code-inside");
            Assert.Equal(30, result.Events.Count);
        }

        [Fact]
        public async Task Crawler_For_Orgs_Should_Embed_The_RawContent_From_The_Event()
        {
            var result = await InvokeSutForOrgs("code-inside");

            var firstResult = result.Events.First();

            var testContent = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(GetTestFileContentFor("org"));

            var firstTestContent = testContent.First.ToString();

            Assert.Equal(firstTestContent, firstResult.RawContent);
        }

        [Fact]
        public async Task Crawler_For_Users_Should_Return_Correct_Number_Of_Events()
        {
            var result = await InvokeSutForUsers("robertmuehsig");
            Assert.Equal(30, result.Events.Count);
        }

        [Fact]
        public async Task Crawler_For_Users_Should_Embed_The_RawContent_From_The_Event()
        {
            var result = await InvokeSutForUsers("robertmuehsig");

            var firstResult = result.Events.First();

            var testContent = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(GetTestFileContentFor("user"));

            var firstTestContent = testContent.First.ToString();

            Assert.Equal(firstTestContent, firstResult.RawContent);
        }

        [Fact]
        public async Task Crawler_For_Repos_Should_Return_Correct_Number_Of_Events()
        {
            var result = await InvokeSutForRepos("aspnet/mvc");
            Assert.Equal(30, result.Events.Count);
        }

        [Fact]
        public async Task Crawler_For_Repos_Should_Embed_The_RawContent_From_The_Event()
        {
            var result = await InvokeSutForRepos("aspnet/mvc");

            var firstResult = result.Events.First();

            var testContent = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(GetTestFileContentFor("repo"));

            var firstTestContent = testContent.First.ToString();

            Assert.Equal(firstTestContent, firstResult.RawContent);
        }


        [Fact]
        public async Task Crawler_Must_Not_Return_Null_If_Nothing_Is_Configured()
        {
            var result = await InvokeSutForOrgs(string.Empty);
            Assert.NotNull(result);
        }
    }
}