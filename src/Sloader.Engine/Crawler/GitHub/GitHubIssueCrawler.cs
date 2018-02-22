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

            var events = JsonConvert.DeserializeObject<JArray>(githubResult);

        }
    }
}
