using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sloader.Shared;
using Sloader.Types;
using WorldDomination.Net.Http;

namespace Sloader.Crawler.Twitter
{
    public class TwitterTimelineCrawler : ICrawler<TwitterTimelineCrawlerResult>
    {

        public TwitterTimelineCrawler()
        {
            Config = new TwitterTimelineCrawlerConfig();
        }

        public TwitterTimelineCrawlerConfig Config { get; set; }

        public async Task<TwitterTimelineCrawlerResult> DoWorkAsync()
        {
            if (string.IsNullOrWhiteSpace(Config.Handle))
                return new TwitterTimelineCrawlerResult();

            var result = new TwitterTimelineCrawlerResult();

            result.Key = Config.Handle;
            result.Type = KnownCrawler.TwitterTimeline;
            result.Tweets = new List<TwitterTimelineCrawlerResult.Tweet>();

            var twitterResult = await GetTwitterTimeline(Config.OAuthToken, Config.Handle);
            result.Tweets.AddRange(new List<TwitterTimelineCrawlerResult.Tweet>(twitterResult));

            return result;
        }

        private static async Task<List<TwitterTimelineCrawlerResult.Tweet>> GetTwitterTimeline(string oauthToken, string screenname)
        {
            Trace.TraceInformation("GetTwitterTimeline invoked with screenname:" + screenname);

            using (var httpClient = HttpClientFactory.GetHttpClient())
            {
                var timelineFormat =
                    "https://api.twitter.com/1.1/statuses/user_timeline.json?screen_name={0}&include_rts=1&exclude_replies=1&count=5";
                var timelineUrl = string.Format(timelineFormat, screenname);

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oauthToken);
                var response = await httpClient.GetAsync(timelineUrl);

                response.EnsureSuccessStatusCode();

                string timeline = await response.Content.ReadAsStringAsync();

                var resultForThisHandle =
                    JsonConvert.DeserializeObject<List<TwitterTimelineCrawlerResult.Tweet>>(timeline);

                return resultForThisHandle;
            }
        }
    }
}