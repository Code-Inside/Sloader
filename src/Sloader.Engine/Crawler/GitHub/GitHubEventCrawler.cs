using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sloader.Config.Crawler.GitHub;
using Sloader.Result;
using Sloader.Result.Types;
using WorldDomination.Net.Http;

namespace Sloader.Engine.Crawler.GitHub
{
    public class GitHubEventCrawler : ICrawler<GitHubEventResult, GitHubEventCrawlerConfig>
    {
        private readonly HttpClient _httpClient;

        public GitHubEventCrawler(){
            _httpClient = new HttpClient();
        }

        public GitHubEventCrawler(FakeHttpMessageHandler messageHandler)
        {
            _httpClient = new HttpClient(messageHandler);
        }

        public async Task<GitHubEventResult> DoWorkAsync(GitHubEventCrawlerConfig config)
        {
            if (string.IsNullOrWhiteSpace(config.Organization) && string.IsNullOrWhiteSpace(config.Repository) && string.IsNullOrWhiteSpace(config.User))
                return new GitHubEventResult();

            var crawlerResult = new GitHubEventResult();
            crawlerResult.Events = new List<GitHubEventResult.Event>();
            
            if (string.IsNullOrWhiteSpace(config.Organization) == false)
            {
                var maybeSplittedOrgs = config.Organization.Split(';');

                foreach (var maybeSplittedOrg in maybeSplittedOrgs)
                {
                    var apiCall = $"https://api.github.com/orgs/{maybeSplittedOrg}/events";

                    await FetchData(apiCall, crawlerResult);
                }
            }

            if (string.IsNullOrWhiteSpace(config.User) == false)
            {
                var maybeSplittedUsers = config.User.Split(';');

                foreach (var maybeSplittedUser in maybeSplittedUsers)
                {
                    var apiCall = $"https://api.github.com/users/{maybeSplittedUser}/events";

                    await FetchData(apiCall, crawlerResult);
                }
            }

            if (string.IsNullOrWhiteSpace(config.Repository) == false)
            {
                var maybeSplittedRepos = config.Repository.Split(';');

                foreach (var maybeSplittedRepo in maybeSplittedRepos)
                {
                    var apiCall = $"https://api.github.com/repos/{maybeSplittedRepo}/events";

                    await FetchData(apiCall, crawlerResult);
                }
            }

            crawlerResult.Events = crawlerResult.Events.OrderByDescending(x => x.CreatedAt).ToList();

            return crawlerResult;
        }

        private async Task FetchData(string apiCall, GitHubEventResult crawlerResult)
        {
            // needed, otherwise GitHub API will return 
            // The server committed a protocol violation. Section=ResponseStatusLine ERROR
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Anything");
            var response = await _httpClient.GetAsync(apiCall);

            response.EnsureSuccessStatusCode();

            string githubResult = await response.Content.ReadAsStringAsync();

            var events = JsonConvert.DeserializeObject<JArray>(githubResult);

            foreach (var gitHubEvent in events)
            {
                var rawJson = gitHubEvent.ToString();


                GitHubEventResult.Event eventObject = new GitHubEventResult.Event();
                eventObject.Id = gitHubEvent["id"].ToObject<string>();
                eventObject.Type = gitHubEvent["type"].ToObject<string>();
                eventObject.Actor = gitHubEvent["actor"]?["login"].ToObject<string>();
                eventObject.Repository = gitHubEvent["repo"]?["name"].ToObject<string>();
                eventObject.Organization = gitHubEvent["org"]?["login"].ToObject<string>();
                eventObject.RelatedAction = gitHubEvent["payload"]?["action"]?.ToObject<string>();


                if (eventObject.Type == "IssuesEvent" || eventObject.Type == "IssueCommentEvent")
                {
                    eventObject.RelatedUrl = gitHubEvent["payload"]?["issue"]?["html_url"]?.ToObject<string>();
                    eventObject.RelatedDescription = gitHubEvent["payload"]?["issue"]?["title"]?.ToObject<string>();
                }

                if (eventObject.Type == "PullRequestEvent")
                {
                    eventObject.RelatedUrl = gitHubEvent["payload"]?["pull_request"]?["html_url"]?.ToObject<string>();
                    eventObject.RelatedDescription = gitHubEvent["payload"]?["pull_request"]?["title"]?.ToObject<string>();
                }

                if (eventObject.Type == "ReleaseEvent")
                {
                    eventObject.RelatedUrl = gitHubEvent["payload"]?["release"]?["html_url"]?.ToObject<string>();
                    eventObject.RelatedDescription = gitHubEvent["payload"]?["release"]?["tag_name"]?.ToObject<string>();
                }

                var eventDate = gitHubEvent["created_at"].ToObject<string>();

                DateTime eventDateAsDate;
                if (DateTime.TryParse(eventDate, out eventDateAsDate))
                {
                    eventObject.CreatedAt = eventDateAsDate;
                }

                eventObject.RawContent = rawJson;
                crawlerResult.Events.Add(eventObject);
            }
        }
    }
}
