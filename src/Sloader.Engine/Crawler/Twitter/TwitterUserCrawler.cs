using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sloader.Config.Crawler.Twitter;
using Sloader.Engine.Crawler.DependencyServices;
using Sloader.Engine.Util;
using Sloader.Result.Types;
using WorldDomination.Net.Http;

namespace Sloader.Engine.Crawler.Twitter
{
    /// <inheritdoc />
    /// <summary>
    /// Crawler for the Twitter User Lookup API:
    /// <para>https://dev.twitter.com/rest/reference/get/users/lookup</para>
    /// </summary>
    public class TwitterUserCrawler : ICrawler<TwitterUserResult, TwitterUserCrawlerConfig>
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Ctor with default dependencies
        /// </summary>
        public TwitterUserCrawler()
        {
            _httpClient = SloaderRunner.HttpClient;
        }

        /// <summary>
        /// Ctor for testing
        /// </summary>
        /// <param name="messageHandler">HttpMessageHandler to simulate any HTTP response</param>
        public TwitterUserCrawler(FakeHttpMessageHandler messageHandler)
        {
            _httpClient = new HttpClient(messageHandler);
        }

        /// <summary>
        /// Needed Token to consume the API
        /// </summary>
        /// <see cref="TwitterOAuthTokenService"/>
        public string OAuthToken { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Actual worker method which will load the user information for the given users.
        /// </summary>
        /// <param name="config">Twitter Screenname etc.</param>
        /// <returns>result for given config data (e.g. follower count etc.)</returns>
        public async Task<TwitterUserResult> DoWorkAsync(TwitterUserCrawlerConfig config)
        {
            if (string.IsNullOrWhiteSpace(config.Handle))
                return new TwitterUserResult();

            Trace.TraceInformation($"{nameof(TwitterUserCrawlerConfig)} loading stuff for '{config.Handle}'");

            var result = new TwitterUserResult {Users = new List<TwitterUserResult.User>()};



            var twitterResult = await GetTwitterUser(OAuthToken, config.Handle, config.IncludeRawContent);
            result.Users.AddRange(new List<TwitterUserResult.User>(twitterResult));

            return result;
        }

        private async Task<List<TwitterUserResult.User>> GetTwitterUser(string oauthToken, string screenname, bool includeRawContent)
        {
            Trace.TraceInformation("GetTwitterUser invoked with screenname:" + screenname);

            var apiCall =
                "https://api.twitter.com/1.1/users/lookup.json?screen_name={0}";
            apiCall = string.Format(apiCall, screenname);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oauthToken);
            var response = await _httpClient.GetAsync(apiCall);

            response.EnsureSuccessStatusCode();

            string twitterResult = await response.Content.ReadAsStringAsync();

            var users = JsonConvert.DeserializeObject<JArray>(twitterResult);
            var resultForThisHandle = new List<TwitterUserResult.User>();

            foreach (var user in users)
            {
                var rawJson = user.ToString();

                TwitterUserResult.User userObject = new TwitterUserResult.User
                {
                    Id = user["id_str"].ToObject<string>(),
                    Name = user["name"].ToObject<string>(),
                    Url = user["url"].ToObject<string>(),
                    FollowersCount = user["followers_count"].ToObject<int>(),
                    Description = user["description"].ToObject<string>().ToCleanString()
                };

                var userDate = user["created_at"].ToObject<string>();

                if (DateTime.TryParseExact(userDate, "ddd MMM dd HH:mm:ss zzz yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var userDateAsDate))
                {
                    userObject.CreatedAt = userDateAsDate;
                }

                if(includeRawContent)
                {
                    userObject.RawContent = rawJson;
                }

                resultForThisHandle.Add(userObject);
            }

            return resultForThisHandle;
        }
    }

}
