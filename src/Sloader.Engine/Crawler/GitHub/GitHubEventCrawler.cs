using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sloader.Config.Crawler.GitHub;
using Sloader.Engine.Util;
using Sloader.Result.Types;
using WorldDomination.Net.Http;

namespace Sloader.Engine.Crawler.GitHub
{
    /// <inheritdoc />
    /// <summary>
    /// GitHub Event Crawler will use this HTTP-API endpoint:
    /// <para>https://developer.github.com/v3/activity/events/</para>
    /// </summary>
    public class GitHubEventCrawler : ICrawler<GitHubEventResult, GitHubEventCrawlerConfig>
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Ctor with default dependencies
        /// </summary>
        public GitHubEventCrawler(){
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Ctor for testing
        /// </summary>
        /// <param name="messageHandler">HttpMessageHandler to simulate any HTTP response</param>
        public GitHubEventCrawler(FakeHttpMessageHandler messageHandler)
        {
            _httpClient = new HttpClient(messageHandler);
        }

        /// <inheritdoc />
        /// <summary>
        /// Actual work method to load event data.
        /// <para>Depending on the different EventTypes, this method will try to get the most helpful description and url from the huge API response.</para>
        /// </summary>
        /// <param name="config">Crawler Config</param>
        /// <returns>result for the given config data</returns>
        public async Task<GitHubEventResult> DoWorkAsync(GitHubEventCrawlerConfig config)
        {
            if (string.IsNullOrWhiteSpace(config.Organization) && string.IsNullOrWhiteSpace(config.Repository) && string.IsNullOrWhiteSpace(config.User))
                return new GitHubEventResult();

            var crawlerResult = new GitHubEventResult { Events = new List<GitHubEventResult.Event>() };

            if (string.IsNullOrWhiteSpace(config.Organization) == false)
            {
                var maybeSplittedOrgs = config.Organization.Split(';');

                foreach (var maybeSplittedOrg in maybeSplittedOrgs)
                {
                    var apiCall = $"https://api.github.com/orgs/{maybeSplittedOrg}/events";

                    await FetchData(apiCall, crawlerResult, config.Events, config.IncludeRawContent);
                }
            }

            if (string.IsNullOrWhiteSpace(config.User) == false)
            {
                var maybeSplittedUsers = config.User.Split(';');

                foreach (var maybeSplittedUser in maybeSplittedUsers)
                {
                    var apiCall = $"https://api.github.com/users/{maybeSplittedUser}/events";

                    await FetchData(apiCall, crawlerResult, config.Events, config.IncludeRawContent);
                }
            }

            if (string.IsNullOrWhiteSpace(config.Repository) == false)
            {
                var maybeSplittedRepos = config.Repository.Split(';');

                foreach (var maybeSplittedRepo in maybeSplittedRepos)
                {
                    var apiCall = $"https://api.github.com/repos/{maybeSplittedRepo}/events";

                    await FetchData(apiCall, crawlerResult, config.Events, config.IncludeRawContent);
                }
            }

            crawlerResult.Events = crawlerResult.Events.OrderByDescending(x => x.CreatedAt).ToList();

            return crawlerResult;
        }

        private async Task FetchData(string apiCall, GitHubEventResult crawlerResult, List<string> includedEvents, bool includeRawContent)
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


                GitHubEventResult.Event eventObject =
                    new GitHubEventResult.Event {Type = gitHubEvent["type"].ToObject<string>()};

                if (includedEvents.Contains("*") == false && includedEvents.Contains(eventObject.Type) == false)
                    continue;

                eventObject.Id = gitHubEvent["id"].ToObject<string>();
                eventObject.Actor = gitHubEvent["actor"]?["login"].ToObject<string>();
                eventObject.Repository = gitHubEvent["repo"]?["name"].ToObject<string>();
                eventObject.Organization = gitHubEvent["org"]?["login"].ToObject<string>();
                eventObject.RelatedAction = gitHubEvent["payload"]?["action"]?.ToObject<string>();

                if (eventObject.Type == "IssuesEvent")
                {
                    eventObject.RelatedUrl = gitHubEvent["payload"]?["issue"]?["html_url"]?.ToObject<string>();
                    eventObject.RelatedBody = gitHubEvent["payload"]?["issue"]?["body"]?.ToObject<string>().ToCleanString();

                    if (eventObject.RelatedAction == "closed")
                    {
                        eventObject.RelatedDescription = "Closed issue \"" + gitHubEvent["payload"]?["issue"]?["title"]?.ToObject<string>().ToCleanString()  + "\" (#" + gitHubEvent["payload"]?["issue"]?["number"]?.ToObject<string>() + ") at " + gitHubEvent["repo"]?["name"]?.ToObject<string>(); 
                    }
                    else if (eventObject.RelatedAction == "reopened")
                    {
                        eventObject.RelatedDescription = "Reopened issue \"" + gitHubEvent["payload"]?["issue"]?["title"]?.ToObject<string>().ToCleanString() + "\" (#" + gitHubEvent["payload"]?["issue"]?["number"]?.ToObject<string>() + ") at " + gitHubEvent["repo"]?["name"]?.ToObject<string>(); 
                    }
                    else if (eventObject.RelatedAction == "opened")
                    {
                        eventObject.RelatedDescription = "Opened issue \"" + gitHubEvent["payload"]?["issue"]?["title"]?.ToObject<string>().ToCleanString() + "\" (#" + gitHubEvent["payload"]?["issue"]?["number"]?.ToObject<string>() + ") at " + gitHubEvent["repo"]?["name"]?.ToObject<string>(); 
                    }

                }
                else if (eventObject.Type == "IssueCommentEvent")
                {
                    eventObject.RelatedUrl = gitHubEvent["payload"]?["comment"]?["html_url"]?.ToObject<string>();
                    eventObject.RelatedBody = gitHubEvent["payload"]?["comment"]?["body"]?.ToObject<string>().ToCleanString();

                    if (eventObject.RelatedAction == "created")
                    {
                        eventObject.RelatedDescription = "Commented on issue \"" + gitHubEvent["payload"]?["issue"]?["title"]?.ToObject<string>().ToCleanString() + "\" (#" + gitHubEvent["payload"]?["issue"]?["number"]?.ToObject<string>() + ") at " + gitHubEvent["repo"]?["name"]?.ToObject<string>();
                    }
                }
                else if (eventObject.Type == "CommitCommentEvent")
                {
                    eventObject.RelatedUrl = gitHubEvent["payload"]?["comment"]?["html_url"]?.ToObject<string>();
                    eventObject.RelatedBody = gitHubEvent["payload"]?["comment"]?["body"]?.ToObject<string>().ToCleanString();
                    eventObject.RelatedDescription = "Commented on commit \"" + gitHubEvent["payload"]?["comment"]?["commit_id"]?.ToObject<string>() + "\" at " + gitHubEvent["repo"]?["name"]?.ToObject<string>();
                }
                else if (eventObject.Type == "WatchEvent")
                {
                    eventObject.RelatedDescription = "Starred " + gitHubEvent["repo"]?["name"]?.ToObject<string>();
                }
                else if (eventObject.Type == "PullRequestEvent")
                {
                    eventObject.RelatedUrl = gitHubEvent["payload"]?["pull_request"]?["html_url"]?.ToObject<string>();
                    eventObject.RelatedBody = gitHubEvent["payload"]?["pull_request"]?["body"]?.ToObject<string>().ToCleanString();

                    if (eventObject.RelatedAction == "closed" && gitHubEvent["payload"]?["pull_request"]?["merged"]?.ToObject<string>().ToLowerInvariant() == "true")
                    {
                        eventObject.RelatedAction = "merged";
                        eventObject.RelatedDescription = "Merged pull request \"" + gitHubEvent["payload"]?["pull_request"]?["title"]?.ToObject<string>().ToCleanString() + "\" (#" + gitHubEvent["payload"]?["pull_request"]?["number"]?.ToObject<string>() + ") at " + gitHubEvent["repo"]?["name"]?.ToObject<string>(); 
                    }
                    else if (eventObject.RelatedAction == "closed" && gitHubEvent["payload"]?["pull_request"]?["merged"]?.ToObject<string>().ToLowerInvariant() == "false")
                    {
                        eventObject.RelatedDescription = "Closed pull request \"" + gitHubEvent["payload"]?["pull_request"]?["title"]?.ToObject<string>().ToCleanString() + "\" (#" + gitHubEvent["payload"]?["pull_request"]?["number"]?.ToObject<string>() + ") at " + gitHubEvent["repo"]?["name"]?.ToObject<string>();
                    }
                    else if (eventObject.RelatedAction == "opened")
                    {
                        eventObject.RelatedDescription = "Opened pull request \"" + gitHubEvent["payload"]?["pull_request"]?["title"]?.ToObject<string>().ToCleanString() + "\" (#" + gitHubEvent["payload"]?["pull_request"]?["number"]?.ToObject<string>() + ") at " + gitHubEvent["repo"]?["name"]?.ToObject<string>();
                    }
                }
                else if (eventObject.Type == "DeleteEvent")
                {
                    eventObject.RelatedDescription = "Deleted " + gitHubEvent["payload"]?["ref_type"]?.ToObject<string>() + " \"" + gitHubEvent["payload"]?["ref"]?.ToObject<string>() + "\" at " + gitHubEvent["repo"]?["name"]?.ToObject<string>();
                }
                else if (eventObject.Type == "CreateEvent")
                {
                    eventObject.RelatedDescription = "Created " + gitHubEvent["payload"]?["ref_type"]?.ToObject<string>() + " \"" + gitHubEvent["payload"]?["ref"]?.ToObject<string>() + "\" at " + gitHubEvent["repo"]?["name"]?.ToObject<string>();
                    if (gitHubEvent["payload"]?["ref_type"]?.ToObject<string>().ToLowerInvariant() == "branch")
                    {
                        eventObject.RelatedUrl = "https://github.com/" + gitHubEvent["repo"]?["name"]?.ToObject<string>() + "/tree/" + gitHubEvent["payload"]?["ref"]?.ToObject<string>();
                    }
                }
                else if (eventObject.Type == "ReleaseEvent")
                {
                    eventObject.RelatedUrl = gitHubEvent["payload"]?["release"]?["html_url"]?.ToObject<string>();
                    eventObject.RelatedBody = gitHubEvent["payload"]?["release"]?["body"]?.ToObject<string>().ToCleanString();
                    eventObject.RelatedDescription = "Released \"" + gitHubEvent["payload"]?["release"]?["tag_name"]?.ToObject<string>() + "\" at " + gitHubEvent["repo"]?["name"]?.ToObject<string>();
                }
                else if (eventObject.Type == "ForkEvent")
                {
                    eventObject.RelatedUrl = gitHubEvent["payload"]?["forkee"]?["html_url"]?.ToObject<string>();
                    eventObject.RelatedDescription = "Forked " + gitHubEvent["repo"]?["name"]?.ToObject<string>() + " to " + gitHubEvent["payload"]?["forkee"]?["full_name"]?.ToObject<string>();
                }
                else if (eventObject.Type == "PushEvent")
                {
                    eventObject.RelatedUrl = "https://github.com/" + gitHubEvent["repo"]?["name"]?.ToObject<string>() + "/compare/" + gitHubEvent["payload"]?["before"]?.ToObject<string>() + "..." + gitHubEvent["payload"]?["head"]?.ToObject<string>();
                    eventObject.RelatedDescription = "Pushed to " + gitHubEvent["payload"]?["ref"]?.ToObject<string>() + " at " + gitHubEvent["repo"]?["name"]?.ToObject<string>();
                }
                else if (eventObject.Type == "PullRequestReviewCommentEvent")
                {
                    eventObject.RelatedUrl = gitHubEvent["payload"]?["comment"]?["html_url"]?.ToObject<string>();
                    eventObject.RelatedBody = gitHubEvent["payload"]?["comment"]?["body"]?.ToObject<string>().ToCleanString();
                    eventObject.RelatedDescription = "Commented on pull request \"" + gitHubEvent["payload"]?["pull_request"]?["title"]?.ToObject<string>().ToCleanString() + "\" (#" + gitHubEvent["payload"]?["pull_request"]?["number"]?.ToObject<string>() + ") at " + gitHubEvent["repo"]?["name"]?.ToObject<string>();
                }

                var eventDate = gitHubEvent["created_at"].ToObject<string>();

                if (DateTime.TryParse(eventDate, out var eventDateAsDate))
                {
                    eventObject.CreatedAt = eventDateAsDate;
                }

                if(includeRawContent)
                {
                    eventObject.RawContent = rawJson;
                }

                crawlerResult.Events.Add(eventObject);
            }
        }
    }
}
