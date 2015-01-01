using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sloader.Types;

namespace Sloader.Bootstrapper.Crawler.Twitter
{
    public class TwitterTimelineCrawler : ICrawler<List<TwitterTimelineCrawlerResult>>
    {
        public TwitterTimelineCrawler()
        {
            Config = new TwitterTimelineCrawlerConfig();
        }

        public TwitterTimelineCrawlerConfig Config { get; set; }

        public async Task<List<TwitterTimelineCrawlerResult>> DoWorkAsync()
        {
            var results = new List<TwitterTimelineCrawlerResult>();

            var oauth = await GetTwitterAccessToken(Config.ConsumerKey, Config.ConsumerSecret);

            foreach (var handle in Config.Handles.Split(';'))
            {
                var result = new TwitterTimelineCrawlerResult();

                result.Key = handle;
                result.Type = KnownCrawler.TwitterTimeline;
                result.Tweets = new List<TwitterTimelineCrawlerResult.Tweet>();

                var twitterResult = await GetTwitterTimeline(oauth, handle);
                result.Tweets.AddRange(new List<TwitterTimelineCrawlerResult.Tweet>(twitterResult));

                results.Add(result);
            }

            return results;
        }

        private static async Task<string> GetTwitterAccessToken(string consumerKey, string consumerSecret)
        {
            var client = new HttpClient();

            var authHeaderParameter = Convert.ToBase64String(Encoding.UTF8.GetBytes(Uri.EscapeDataString(consumerKey) + ":" +
                                                                                    Uri.EscapeDataString((consumerSecret))));

            var postBody = "grant_type=client_credentials";

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderParameter);

            var response = await client.PostAsync("https://api.twitter.com/oauth2/token", new StringContent(postBody, Encoding.UTF8, "application/x-www-form-urlencoded"));

            response.EnsureSuccessStatusCode();

            string oauthtoken = await response.Content.ReadAsStringAsync();
            var jToken = JToken.Parse(oauthtoken);
            var accessToken = jToken.SelectToken("access_token");

            return accessToken.Value<string>();
        }

        private static async Task<List<TwitterTimelineCrawlerResult.Tweet>> GetTwitterTimeline(string oauthToken, string screenname)
        {
            Trace.TraceInformation("GetTwitterTimeline invoked with screenname:" + screenname);

            var client = new HttpClient();

            var timelineFormat = "https://api.twitter.com/1.1/statuses/user_timeline.json?screen_name={0}&include_rts=1&exclude_replies=1&count=5";
            var timelineUrl = string.Format(timelineFormat, screenname);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oauthToken);
            var response = await client.GetAsync(timelineUrl);

            response.EnsureSuccessStatusCode();

            string timeline = await response.Content.ReadAsStringAsync();

            var resultForThisHandle = JsonConvert.DeserializeObject<List<TwitterTimelineCrawlerResult.Tweet>>(timeline);

            return resultForThisHandle;
        }
    }
}