using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sloader.Config.Crawler.GitHub;
using Sloader.Result.Types;
using WorldDomination.Net.Http;

namespace Sloader.Engine.Crawler.GitHub
{
    /// <inheritdoc />
    /// <summary>
    /// GitHub Issue Crawler will use this HTTP-API endpoint:
    /// <para>https://developer.github.com/v3/issues/#list-issues-for-a-repository</para>
    /// </summary>
    public class GitHubIssueCrawler : ICrawler<GitHubIssueResult, GitHubIssueCrawlerConfig>
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Ctor with default dependencies
        /// </summary>
        public GitHubIssueCrawler(){
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Ctor for testing
        /// </summary>
        /// <param name="messageHandler">HttpMessageHandler to simulate any HTTP response</param>
        public GitHubIssueCrawler(FakeHttpMessageHandler messageHandler)
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
        public async Task<GitHubIssueResult> DoWorkAsync(GitHubIssueCrawlerConfig config)
        {
            if (string.IsNullOrWhiteSpace(config.Repository))
                return new GitHubIssueResult();

            var crawlerResult = new GitHubIssueResult();
            crawlerResult.Issues = new List<GitHubIssueResult.Issue>();

            if (string.IsNullOrWhiteSpace(config.Repository) == false)
            {
                var maybeSplittedRepos = config.Repository.Split(';');

                foreach (var maybeSplittedRepo in maybeSplittedRepos)
                {
                    var apiCall = $"https://api.github.com/repos/{maybeSplittedRepo}/issues";

                    await FetchData(apiCall, crawlerResult, config.IncludeRawContent);
                }
            }

            crawlerResult.Issues = crawlerResult.Issues.OrderByDescending(x => x.CreatedAt).ToList();

            return crawlerResult;
        }

        private async Task FetchData(string apiCall, GitHubIssueResult crawlerResult, bool includeRawContent)
        {
            // needed, otherwise GitHub API will return 
            // The server committed a protocol violation. Section=ResponseStatusLine ERROR
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Anything");
            var response = await _httpClient.GetAsync(apiCall);

            response.EnsureSuccessStatusCode();

            string githubResult = await response.Content.ReadAsStringAsync();

            var issues = JsonConvert.DeserializeObject<JArray>(githubResult);

            foreach (var gitHubIssue in issues)
            {
                var rawJson = gitHubIssue.ToString();


                GitHubIssueResult.Issue issueObject = new GitHubIssueResult.Issue();

                issueObject.Id = gitHubIssue["id"].ToObject<string>();
                issueObject.Number = gitHubIssue["number"].ToObject<string>();
                issueObject.State = gitHubIssue["state"].ToObject<string>();
                issueObject.Title = gitHubIssue["title"].ToObject<string>();
                issueObject.Body = gitHubIssue["body"].ToObject<string>();

                issueObject.Actor = gitHubIssue["user"]?["login"].ToObject<string>();

                if (gitHubIssue["pull_request"] != null)
                {
                    issueObject.IsPullRequest = true;
                    issueObject.Url = gitHubIssue["pull_request"]["html_url"].ToObject<string>();
                    eventObject.RelatedUrl = gitHubEvent["payload"]?["issue"]?["html_url"]?.ToObject<string>();
                    eventObject.RelatedBody = gitHubEvent["payload"]?["issue"]?["body"]?.ToObject<string>();

                    if (eventObject.RelatedAction == "closed")
                    {
                        eventObject.RelatedDescription = "Closed issue \"" + gitHubEvent["payload"]?["issue"]?["title"]?.ToObject<string>() + "\" (#" + gitHubEvent["payload"]?["issue"]?["number"]?.ToObject<string>() + ") at " + gitHubEvent["repo"]?["name"]?.ToObject<string>();
                    }
                    else if (eventObject.RelatedAction == "reopened")
                    {
                        eventObject.RelatedDescription = "Reopened issue \"" + gitHubEvent["payload"]?["issue"]?["title"]?.ToObject<string>() + "\" (#" + gitHubEvent["payload"]?["issue"]?["number"]?.ToObject<string>() + ") at " + gitHubEvent["repo"]?["name"]?.ToObject<string>();
                    }
                    else if (eventObject.RelatedAction == "opened")
                    {
                        eventObject.RelatedDescription = "Opened issue \"" + gitHubEvent["payload"]?["issue"]?["title"]?.ToObject<string>() + "\" (#" + gitHubEvent["payload"]?["issue"]?["number"]?.ToObject<string>() + ") at " + gitHubEvent["repo"]?["name"]?.ToObject<string>();
                    }

                }
                else if (eventObject.Type == "IssueCommentEvent")
                {
                    eventObject.RelatedUrl = gitHubEvent["payload"]?["comment"]?["html_url"]?.ToObject<string>();
                    eventObject.RelatedBody = gitHubEvent["payload"]?["comment"]?["body"]?.ToObject<string>();

                    if (eventObject.RelatedAction == "created")
                    {
                        eventObject.RelatedDescription = "Commented on issue \"" + gitHubEvent["payload"]?["issue"]?["title"]?.ToObject<string>() + "\" (#" + gitHubEvent["payload"]?["issue"]?["number"]?.ToObject<string>() + ") at " + gitHubEvent["repo"]?["name"]?.ToObject<string>();
                    }
                }
                else if (eventObject.Type == "CommitCommentEvent")
                {
                    eventObject.RelatedUrl = gitHubEvent["payload"]?["comment"]?["html_url"]?.ToObject<string>();
                    eventObject.RelatedBody = gitHubEvent["payload"]?["comment"]?["body"]?.ToObject<string>();
                    eventObject.RelatedDescription = "Commented on commit \"" + gitHubEvent["payload"]?["comment"]?["commit_id"]?.ToObject<string>() + "\" at " + gitHubEvent["repo"]?["name"]?.ToObject<string>();
                }
                else if (eventObject.Type == "WatchEvent")
                {
                    eventObject.RelatedDescription = "Starred " + gitHubEvent["repo"]?["name"]?.ToObject<string>();
                }
                else if (eventObject.Type == "PullRequestEvent")
                {
                    eventObject.RelatedUrl = gitHubEvent["payload"]?["pull_request"]?["html_url"]?.ToObject<string>();
                    eventObject.RelatedBody = gitHubEvent["payload"]?["pull_request"]?["body"]?.ToObject<string>();

                    if (eventObject.RelatedAction == "closed" && gitHubEvent["payload"]?["pull_request"]?["merged"]?.ToObject<string>().ToLowerInvariant() == "true")
                    {
                        eventObject.RelatedAction = "merged";
                        eventObject.RelatedDescription = "Merged pull request \"" + gitHubEvent["payload"]?["pull_request"]?["title"]?.ToObject<string>() + "\" (#" + gitHubEvent["payload"]?["pull_request"]?["number"]?.ToObject<string>() + ") at " + gitHubEvent["repo"]?["name"]?.ToObject<string>();
                    }
                    else if (eventObject.RelatedAction == "closed" && gitHubEvent["payload"]?["pull_request"]?["merged"]?.ToObject<string>().ToLowerInvariant() == "false")
                    {
                        eventObject.RelatedDescription = "Closed pull request \"" + gitHubEvent["payload"]?["pull_request"]?["title"]?.ToObject<string>() + "\" (#" + gitHubEvent["payload"]?["pull_request"]?["number"]?.ToObject<string>() + ") at " + gitHubEvent["repo"]?["name"]?.ToObject<string>();
                    }
                    else if (eventObject.RelatedAction == "opened")
                    {
                        eventObject.RelatedDescription = "Opened pull request \"" + gitHubEvent["payload"]?["pull_request"]?["title"]?.ToObject<string>() + "\" (#" + gitHubEvent["payload"]?["pull_request"]?["number"]?.ToObject<string>() + ") at " + gitHubEvent["repo"]?["name"]?.ToObject<string>();
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
                    eventObject.RelatedBody = gitHubEvent["payload"]?["release"]?["body"]?.ToObject<string>();
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
                    eventObject.RelatedBody = gitHubEvent["payload"]?["comment"]?["body"]?.ToObject<string>();
                    eventObject.RelatedDescription = "Commented on pull request \"" + gitHubEvent["payload"]?["pull_request"]?["title"]?.ToObject<string>() + "\" (#" + gitHubEvent["payload"]?["pull_request"]?["number"]?.ToObject<string>() + ") at " + gitHubEvent["repo"]?["name"]?.ToObject<string>();
                }

                var eventDate = gitHubEvent["created_at"].ToObject<string>();

                DateTime eventDateAsDate;
                if (DateTime.TryParse(eventDate, out eventDateAsDate))
                {
                    eventObject.CreatedAt = eventDateAsDate;
                }

                if (includeRawContent)
                {
                    eventObject.RawContent = rawJson;
                }

                crawlerResult.Events.Add(eventObject);
            }
        }
    }
}
