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
            string responseData = string.Empty;

            if (user == "robertmuehsig")
            {
                responseData = GetTestFileContentFor("user_1");
            }
            else if (user == "adamralph")
            {
                responseData = GetTestFileContentFor("user_2");
            }
            else if (user == "davidfowl")
            {
                responseData = GetTestFileContentFor("user_3");
            }
            else if (user == "ryuyu")
            {
                responseData = GetTestFileContentFor("user_4");
            }
            else if (user == "jacqueswww")
            {
                responseData = GetTestFileContentFor("user_5");
            }

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

            var testContent = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(GetTestFileContentFor("user_1"));

            var firstTestContent = testContent.First.ToString();

            Assert.Equal(firstTestContent, firstResult.RawContent);
        }

        [Fact]
        public async Task Crawler_Should_Return_CorrectRelatedData_For_PushEvents()
        {
            var result = await InvokeSutForUsers("robertmuehsig");
            var targetEvent = result.Events.Single(x => x.Id == "5351207656");
            Assert.Equal("https://github.com/Code-Inside/Sloader/compare/a75f750a9911a6d9945ea187b63abcf2fb232568...6738c8d3a313c5a1f6f36637ac01fd7a85886196", targetEvent.RelatedUrl);
            Assert.Equal("Pushed to refs/heads/new-start at Code-Inside/Sloader", targetEvent.RelatedDescription);
        }

        [Fact]
        public async Task Crawler_Should_Return_CorrectRelatedData_For_IssueEvents_For_Closed()
        {
            var result = await InvokeSutForUsers("robertmuehsig");
            var targetEvent = result.Events.Single(x => x.Id == "5351196052");
            Assert.Equal("https://github.com/Code-Inside/Sloader/issues/13", targetEvent.RelatedUrl);
            Assert.Equal("Closed issue \"Handle output in sloader\" (#13) at Code-Inside/Sloader", targetEvent.RelatedDescription);
            Assert.Equal("Instead of using the azure webjobs sdk sloader should save the output directly\n", targetEvent.RelatedBody);
        }

        [Fact]
        public async Task Crawler_Should_Return_CorrectRelatedData_For_IssueEvents_For_Reopened()
        {
            var result = await InvokeSutForRepos("aspnet/Mvc");
            var targetEvent = result.Events.Single(x => x.Id == "5348037405");
            Assert.Equal("https://github.com/aspnet/Mvc/issues/5811", targetEvent.RelatedUrl);
            Assert.Equal("Reopened issue \"Version numbers in rel/1.1.1 were not updated to 1.1.1\" (#5811) at aspnet/Mvc", targetEvent.RelatedDescription);
            Assert.Equal("The version numbers are still 1.1.0. Should also make sure all the changes are up to date and contains all the fixes for 1.1.1\r\n\r\ncc @pranavkm ", targetEvent.RelatedBody);
        }

        [Fact]
        public async Task Crawler_Should_Return_CorrectRelatedData_For_IssueEvents_For_Opened()
        {
            var result = await InvokeSutForUsers("adamralph");
            var targetEvent = result.Events.Single(x => x.Id == "5405757298");
            Assert.Equal("https://github.com/Particular/NServiceBus.RabbitMQ/issues/347", targetEvent.RelatedUrl);
            Assert.Equal("Opened issue \"4.3.0 release\" (#347) at Particular/NServiceBus.RabbitMQ", targetEvent.RelatedDescription);
            Assert.Equal("- [ ] Beta testing (1 week to get any response, total max 2 weeks for Beta testing)\r\n  - [ ] Package to be on MyGet\r\n  - [ ] Ask TAMs if they have customers on NSB v6 and RabbitMQ that would be interested in beta testing\r\n  - [ ] Announce on Google Groups\r\n- [ ] Follow regular release procedure\r\n- [ ] Merge https://github.com/Particular/docs.particular.net/pull/2366\r\n- [ ] Merge https://github.com/Particular/docs.particular.net/pull/2451", targetEvent.RelatedBody);
        }

        [Fact]
        public async Task Crawler_Should_Return_CorrectRelatedData_For_PullRequestEvent_For_Open()
        {
            var result = await InvokeSutForUsers("ryuyu");
            var targetEvent = result.Events.Single(x => x.Id == "5388840207");
            Assert.Equal("https://github.com/NuGet/NuGetGallery/pull/3597", targetEvent.RelatedUrl);
            Assert.Equal("Opened pull request \"Merging Dev into Master\" (#3597) at NuGet/NuGetGallery", targetEvent.RelatedDescription);
            Assert.Equal("Merging dev into master in preparation of the 2017.02.24 release\r\n\r\n@joelverhagen @chenriksson @shishirx34 @scottbommarito @skofman1 @dtivel ", targetEvent.RelatedBody);
        }

        [Fact]
        public async Task Crawler_Should_Return_CorrectRelatedData_For_PullRequestEvent_For_Closed_Merged()
        {
            var result = await InvokeSutForUsers("adamralph");
            var targetEvent = result.Events.Single(x => x.Id == "5405598530");
            Assert.Equal("https://github.com/Particular/NServiceBus.RabbitMQ/pull/346", targetEvent.RelatedUrl);
            Assert.Equal("Merged pull request \"Remove confirmation id header\" (#346) at Particular/NServiceBus.RabbitMQ", targetEvent.RelatedDescription);
            Assert.Equal("Fixes #345\r\n\r\nThe branch is currently based against the #329 branch, will rebase to develop once it has been merged.", targetEvent.RelatedBody);
        }

        [Fact]
        public async Task Crawler_Should_Return_CorrectRelatedData_For_PullRequestEvent_For_Closed_Close()
        {
            var result = await InvokeSutForUsers("davidfowl");
            var targetEvent = result.Events.Single(x => x.Id == "5403268816");
            Assert.Equal("https://github.com/aspnet/KestrelHttpServer/pull/1406", targetEvent.RelatedUrl);
            Assert.Equal("Closed pull request \"Added test for query string without path\" (#1406) at aspnet/KestrelHttpServer", targetEvent.RelatedDescription);
            Assert.Equal("/cc @halter73 @cesarbs ", targetEvent.RelatedBody);

        }

        [Fact]
        public async Task Crawler_Should_Return_CorrectRelatedData_For_CreateEvent()
        {
            var result = await InvokeSutForUsers("ryuyu");
            var targetEvent = result.Events.Single(x => x.Id == "5389029733");
            Assert.Equal("Created tag \"v2017.02.24\" at NuGet/NuGet.Services.Metadata", targetEvent.RelatedDescription);
        }

        [Fact]
        public async Task Crawler_Should_Return_CorrectRelatedData_For_ForkEvent()
        {
            var result = await InvokeSutForUsers("jacqueswww");
            var targetEvent = result.Events.Single(x => x.Id == "4956614974");
            Assert.Equal("Forked jazzband/django-debug-toolbar to jacqueswww/django-debug-toolbar", targetEvent.RelatedDescription);
        }

        [Fact]
        public async Task Crawler_Should_Return_CorrectRelatedData_For_ReleaseEvent()
        {
            var result = await InvokeSutForUsers("ryuyu");
            var targetEvent = result.Events.Single(x => x.Id == "5389029718");
            Assert.Equal("https://github.com/NuGet/NuGet.Services.Metadata/releases/tag/v2017.02.24", targetEvent.RelatedUrl);
            Assert.Equal("Released \"v2017.02.24\" at NuGet/NuGet.Services.Metadata", targetEvent.RelatedDescription);
            Assert.Equal(" Changelog:\r\n - Adding AI Exception tracking (#119)  …\t\t\t5f596ee\r\n - Export functional test results as XML and add Configure-FunctionalTes…  …\t\t\tdf98289\r\n - update tests\\.nuget\\packages.config (#120)\t\t\t9b4c28e\r\n - Set XmlResolver outside initializer to resolve CodeAnalysis false ala…  …\t\t\t0a828de\r\n - Sync dev to master (#126)  …\t\t\tf9057f1", targetEvent.RelatedBody);
        }

        [Fact]
        public async Task Crawler_Should_Return_CorrectRelatedData_For_DeleteEvent()
        {
            var result = await InvokeSutForUsers("davidfowl");
            var targetEvent = result.Events.Single(x => x.Id == "5402644363");
            Assert.Equal("Deleted branch \"pakrym/ipipe\" at dotnet/corefxlab", targetEvent.RelatedDescription);
        }

        [Fact]
        public async Task Crawler_Should_Return_CorrectRelatedData_For_IssueCommentEvent_For_Created()
        {
            var result = await InvokeSutForUsers("robertmuehsig");
            var targetEvent = result.Events.Single(x => x.Id == "5351196049");
            Assert.Equal("https://github.com/Code-Inside/Sloader/issues/13#issuecomment-280912966", targetEvent.RelatedUrl);
            Assert.Equal("Commented on issue \"Handle output in sloader\" (#13) at Code-Inside/Sloader", targetEvent.RelatedDescription);
            Assert.Equal("Done.", targetEvent.RelatedBody);

        }

        [Fact]
        public async Task Crawler_Should_Return_CorrectRelatedData_For_PullRequestReviewCommentEvent_For_Created()
        {
            var result = await InvokeSutForUsers("davidfowl");
            var targetEvent = result.Events.Single(x => x.Id == "5406710243");
            Assert.Equal("https://github.com/aspnet/KestrelHttpServer/pull/1408#discussion_r103521563", targetEvent.RelatedUrl);
            Assert.Equal("Commented on pull request \"Add an option to Kestrel to disable threadpool dispatching\" (#1408) at aspnet/KestrelHttpServer", targetEvent.RelatedDescription);
            Assert.Equal("Remove ", targetEvent.RelatedBody);

        }

        [Fact]
        public async Task Crawler_Should_Return_CorrectRelatedData_For_WatchEvent_For_Started()
        {
            var result = await InvokeSutForRepos("aspnet/mvc");
            var targetEvent = result.Events.Single(x => x.Id == "5349932726");
            Assert.Equal("Starred aspnet/Mvc", targetEvent.RelatedDescription);
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