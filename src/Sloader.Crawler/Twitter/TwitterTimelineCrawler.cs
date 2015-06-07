using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class TwitterTimelineCrawler : ICrawler<TwitterTimelineCrawlerResult, TwitterTimelineCrawlerConfig>, IDisposable
    {
        private readonly HttpMessageHandler _messageHandler;

        public TwitterTimelineCrawler()
        {
               _messageHandler = new HttpClientHandler();
        }

        public TwitterTimelineCrawler(HttpMessageHandler messageHandler)
        {
            _messageHandler = messageHandler;
        }
        public string OAuthToken { get; set; }

        public async Task<TwitterTimelineCrawlerResult> DoWorkAsync(TwitterTimelineCrawlerConfig config)
        {
            if (string.IsNullOrWhiteSpace(config.Handle))
                return new TwitterTimelineCrawlerResult();

            var result = new TwitterTimelineCrawlerResult();

            result.ResultIdentifier = config.ResultIdentifier;
            result.Tweets = new List<TwitterTimelineCrawlerResult.Tweet>();

            var twitterResult = await GetTwitterTimeline(OAuthToken, config.Handle);
            result.Tweets.AddRange(new List<TwitterTimelineCrawlerResult.Tweet>(twitterResult));

            return result;
        }

        private async Task<List<TwitterTimelineCrawlerResult.Tweet>> GetTwitterTimeline(string oauthToken, string screenname)
        {
            Trace.TraceInformation("GetTwitterTimeline invoked with screenname:" + screenname);

            using (var httpClient = HttpClientFactory.GetHttpClient(_messageHandler, false))
            {
                var timelineFormat =
                    "https://api.twitter.com/1.1/statuses/user_timeline.json?screen_name={0}&include_rts=1&exclude_replies=1&count=5";
                var timelineUrl = string.Format(timelineFormat, screenname);

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oauthToken);
                var response = await httpClient.GetAsync(timelineUrl);

                response.EnsureSuccessStatusCode();

                string timeline = await response.Content.ReadAsStringAsync();

                var tweets = JsonConvert.DeserializeObject<JArray>(timeline);
                var resultForThisHandle = new List<TwitterTimelineCrawlerResult.Tweet>();

                foreach (var tweet in tweets)
                {
                    var rawJson = tweet.ToString(); 
                    var tweetResult = JsonConvert.DeserializeObject<TwitterTimelineCrawlerResult.Tweet>(rawJson);
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