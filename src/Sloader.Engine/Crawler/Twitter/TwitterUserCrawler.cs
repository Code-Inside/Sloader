using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sloader.Config.Crawler.Twitter;
using Sloader.Result;
using Sloader.Result.Types;
using WorldDomination.Net.Http;

namespace Sloader.Engine.Crawler.Twitter
{
    public class TwitterUserCrawler : ICrawler<TwitterUserResult, TwitterUserCrawlerConfig>
    {
        private readonly HttpClient _httpClient;

        public TwitterUserCrawler()
        {
            _httpClient = new HttpClient();
        }

        public TwitterUserCrawler(FakeHttpMessageHandler messageHandler)
        {
            _httpClient = new HttpClient(messageHandler);
        }
        public string OAuthToken { get; set; }

        public async Task<TwitterUserResult> DoWorkAsync(TwitterUserCrawlerConfig config)
        {
            if (string.IsNullOrWhiteSpace(config.Handle))
                return new TwitterUserResult();

            var result = new TwitterUserResult();

            result.Users = new List<TwitterUserResult.User>();


            var twitterResult = await GetTwitterUser(OAuthToken, config.Handle);
            result.Users.AddRange(new List<TwitterUserResult.User>(twitterResult));

            return result;
        }

        private async Task<List<TwitterUserResult.User>> GetTwitterUser(string oauthToken, string screenname)
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
                var tweetResult = JsonConvert.DeserializeObject<TwitterUserResult.User>(rawJson);
                tweetResult.RawContent = rawJson;
                resultForThisHandle.Add(tweetResult);
            }

            return resultForThisHandle;
        }
    }

}
