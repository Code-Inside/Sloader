using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Sloader.Config.Crawler.GitHub;
using Sloader.Engine.Crawler.GitHub;
using Sloader.Result.Types;
using WorldDomination.Net.Http;
using Xunit;

namespace Sloader.Engine.Tests.GitHubIssueCrawlerTests
{
    public class GitHubIssueCrawlerTests
    {
        private static string GetTestFileContent(string file)
        {
            string responseData = TestHelperForCurrentProject.GetTestFileContent($"GitHubIssueCrawlerTests/Sample/repo_{file}.json");

            return responseData;
        }

        private static async Task<GitHubIssueResult> InvokeSut(string repo, string stateFilter = "", bool includeRaw = false)
        {
            string responseData;

            switch (repo)
            {
                case "aspnet/mvc": responseData = GetTestFileContent("mvc");
                    break;
                default: responseData = GetTestFileContent("electronnet");
                    break;
            }

            var messageResponse = FakeHttpMessageHandler.GetStringHttpResponseMessage(responseData);


            string uri = $"https://api.github.com/repos/{repo}/issues";

            if (stateFilter != string.Empty)
            {
                uri = uri + "?state=" + stateFilter;
            }

            var fakeMessageHandler = new FakeHttpMessageHandler(new HttpMessageOptions() { HttpResponseMessage = messageResponse, RequestUri = new System.Uri(uri) });

            var sut = new GitHubIssueCrawler(fakeMessageHandler);

            var config = new GitHubIssueCrawlerConfig
            {
                Repository = repo,
                FilterByState = stateFilter,
                IncludeRawContent = includeRaw
            };
            var result = await sut.DoWorkAsync(config);
            return result;
        }

        [Fact]
        public async Task Crawler_Should_Return_Correct_Number_Of_Issues()
        {
            var result = await InvokeSut("electronnet/electron.net");
            Assert.Equal(29, result.Issues.Count);
        }

        [Fact]
        public async Task Crawler_Should_Return_CorrectRelatedData_For_Open_Issue()
        {
            var result = await InvokeSut("electronnet/electron.net");
            var targetEvent = result.Issues.Single(x => x.Id == "276802368");
            Assert.False(targetEvent.IsPullRequest);
            Assert.Equal("Open issue \"OSX processes fail to shut down on close\" (#68)", targetEvent.RelatedDescription);
        }

        [Fact]
        public async Task Crawler_Should_Return_CorrectRelatedData_For_Closed_PR()
        {
            var result = await InvokeSut("aspnet/mvc", "all");
            var targetEvent = result.Issues.Single(x => x.Id == "298847747");
            Assert.True(targetEvent.IsPullRequest);
            Assert.Equal("Closed or merged PR \"Update README.md\" (#7394)", targetEvent.RelatedDescription);
        }

        [Fact]
        public async Task Crawler_Should_Return_CorrectRelatedData_For_Open_PR()
        {
            var result = await InvokeSut("electronnet/electron.net");
            var targetEvent = result.Issues.Single(x => x.Id == "298418190");
            Assert.True(targetEvent.IsPullRequest);
            Assert.Equal("Open PR \"https://github.com/ElectronNET/Electron.NET/issues/72\" (#101)", targetEvent.RelatedDescription);
        }

    }
}