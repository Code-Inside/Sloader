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
                    if (config.FilterByState != string.Empty)
                    {
                        apiCall = apiCall + "?state=" + config.FilterByState;
                    }
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

                    if (issueObject.State == "closed")
                    {
                        issueObject.RelatedDescription = "Closed or merged PR \"" +
                                                         gitHubIssue["title"]?.ToObject<string>() + "\" (#" +
                                                         gitHubIssue["number"]?.ToObject<string>() + ")";
                    }
                    else if (issueObject.State == "open")
                    {
                        issueObject.RelatedDescription = "Open PR \"" + gitHubIssue["title"]?.ToObject<string>() +
                                                         "\" (#" + gitHubIssue["number"]?.ToObject<string>() + ")";
                    }
                }
                else
                {
                    issueObject.IsPullRequest = false;
                    issueObject.Url = gitHubIssue["html_url"].ToObject<string>();

                    if (issueObject.State == "closed")
                    {
                        issueObject.RelatedDescription = "Closed issue \"" +
                                                         gitHubIssue["title"]?.ToObject<string>() + "\" (#" +
                                                         gitHubIssue["number"]?.ToObject<string>() + ")";
                    }
                    else if (issueObject.State == "open")
                    {
                        issueObject.RelatedDescription = "Open issue \"" + gitHubIssue["title"]?.ToObject<string>() +
                                                         "\" (#" + gitHubIssue["number"]?.ToObject<string>() + ")";
                    }
                }

                string eventDate = string.Empty;

                if (issueObject.State == "closed")
                {
                    eventDate = gitHubIssue["closed_at"].ToObject<string>();

                }
                else if (issueObject.State == "open")
                {
                    eventDate = gitHubIssue["created_at"].ToObject<string>();
                }


                DateTime eventDateAsDate;
                if (DateTime.TryParse(eventDate, out eventDateAsDate))
                {
                    issueObject.CreatedAt = eventDateAsDate;
                }

                if (includeRawContent)
                {
                    issueObject.RawContent = rawJson;
                }

                crawlerResult.Issues.Add(issueObject);
            }
        }
    }
}
