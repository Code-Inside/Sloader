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

            Trace.TraceInformation($"{nameof(TwitterUserCrawlerConfig)} loading stuff for '{config.Handle}'");

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

                TwitterUserResult.User userObject = new TwitterUserResult.User();
                userObject.Id = user["id_str"].ToObject<string>();
                userObject.Name = user["name"].ToObject<string>();
                userObject.Url = user["url"].ToObject<string>();
                userObject.FollowersCount = user["followers_count"].ToObject<int>();
                userObject.Description = user["description"].ToObject<string>();

                var userDate = user["created_at"].ToObject<string>();

                DateTime userDateAsDate;
                if (DateTime.TryParseExact(userDate, "ddd MMM dd HH:mm:ss zzz yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out userDateAsDate))
                {
                    userObject.CreatedAt = userDateAsDate;
                }

                userObject.RawContent = rawJson;
                resultForThisHandle.Add(userObject);
            }

            return resultForThisHandle;
        }
    }

}
