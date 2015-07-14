using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sloader.Crawler.Config;
using Sloader.Crawler.Config.Twitter;
using Sloader.Results;
using WorldDomination.Net.Http;

namespace Sloader.Crawler.Twitter
{
    public class TwitterUserCrawler : ICrawler<TwitterUserCrawlerResult, TwitterUserCrawlerConfig>, IDisposable
    {
        private readonly HttpMessageHandler _messageHandler;

        public TwitterUserCrawler()
        {
               _messageHandler = new HttpClientHandler();
        }

        public TwitterUserCrawler(HttpMessageHandler messageHandler)
        {
            _messageHandler = messageHandler;
        }
        public string OAuthToken { get; set; }

        public async Task<TwitterUserCrawlerResult> DoWorkAsync(TwitterUserCrawlerConfig config)
        {
            if (string.IsNullOrWhiteSpace(config.Handle))
                return new TwitterUserCrawlerResult();

            var result = new TwitterUserCrawlerResult();

            result.ResultIdentifier = config.ResultIdentifier;
            result.Users = new List<TwitterUserCrawlerResult.User>();


            var twitterResult = await GetTwitterUser(OAuthToken, config.Handle);
            result.Users.AddRange(new List<TwitterUserCrawlerResult.User>(twitterResult));

            return result;
        }

        private async Task<List<TwitterUserCrawlerResult.User>> GetTwitterUser(string oauthToken, string screenname)
        {
            Trace.TraceInformation("GetTwitterUser invoked with screenname:" + screenname);

            using (var httpClient = HttpClientFactory.GetHttpClient(_messageHandler, false))
            {
                var apiCall =
                    "https://api.twitter.com/1.1/users/lookup.json?screen_name={0}";
                apiCall = string.Format(apiCall, screenname);

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oauthToken);
                var response = await httpClient.GetAsync(apiCall);

                response.EnsureSuccessStatusCode();

                string twitterResult = await response.Content.ReadAsStringAsync();

                var users = JsonConvert.DeserializeObject<JArray>(twitterResult);
                var resultForThisHandle = new List<TwitterUserCrawlerResult.User>();

                foreach (var user in users)
                {
                    var rawJson = user.ToString();
                    var tweetResult = JsonConvert.DeserializeObject<TwitterUserCrawlerResult.User>(rawJson);
                    tweetResult.RawContent = rawJson;
                    resultForThisHandle.Add(tweetResult);
                }

                return resultForThisHandle;
            }
        }

        public void Dispose()
        {
            _messageHandler.Dispose();
        }
    }
}