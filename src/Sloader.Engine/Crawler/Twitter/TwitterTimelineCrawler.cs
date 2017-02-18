using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
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
    public class TwitterTimelineCrawler : ICrawler<TwitterTimelineResult, TwitterTimelineCrawlerConfig>
    {
        private readonly HttpClient _httpClient;

        public TwitterTimelineCrawler()
        {
            _httpClient = new HttpClient();
        }

        public TwitterTimelineCrawler(FakeHttpMessageHandler messageHandler)
        {
            _httpClient = new HttpClient(messageHandler);
        }

        public string OAuthToken { get; set; }

        public async Task<TwitterTimelineResult> DoWorkAsync(TwitterTimelineCrawlerConfig config)
        {
            if (string.IsNullOrWhiteSpace(config.Handle))
                return new TwitterTimelineResult();

            Trace.TraceInformation($"{nameof(TwitterTimelineCrawler)} loading stuff for '{config.Handle}'");

            var result = new TwitterTimelineResult();

            result.Tweets = new List<TwitterTimelineResult.Tweet>();

            var handleArrayForMultipleHandles = config.Handle.Split(';');

            foreach (var handleArrayForMultipleHandle in handleArrayForMultipleHandles)
            {
                var twitterResult = await GetTwitterTimeline(OAuthToken, handleArrayForMultipleHandle.Trim(), config.IncludeRetweets);
                result.Tweets.AddRange(new List<TwitterTimelineResult.Tweet>(twitterResult));
            }

            result.Tweets = result.Tweets.OrderByDescending(x => x.CreatedAt).ToList();
         
            return result;
        }

        private async Task<List<TwitterTimelineResult.Tweet>> GetTwitterTimeline(string oauthToken, string screenname, bool includeRetweets)
        {
            Trace.TraceInformation("GetTwitterTimeline invoked with screenname:" + screenname);



            var timelineFormat =
                "https://api.twitter.com/1.1/statuses/user_timeline.json?screen_name={0}&include_rts={1}&exclude_replies=1";
            var timelineUrl = string.Format(timelineFormat, screenname, Convert.ToInt32(includeRetweets));

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oauthToken);
            var response = await _httpClient.GetAsync(timelineUrl);
            var resultForThisHandle = new List<TwitterTimelineResult.Tweet>();

            if (response.IsSuccessStatusCode)
            {
                string timeline = await response.Content.ReadAsStringAsync();

                var tweets = JsonConvert.DeserializeObject<JArray>(timeline);

                foreach (var tweet in tweets)
                {
                    var rawJson = tweet.ToString();


                    JObject tweetJson = JObject.Parse(rawJson);

                    TwitterTimelineResult.Tweet tweetObject = new TwitterTimelineResult.Tweet();
                    tweetObject.Id = tweetJson["id_str"].ToObject<string>();
                    tweetObject.Text = tweetJson["text"].ToObject<string>();
                    tweetObject.Source = tweetJson["source"].ToObject<string>();
                    tweetObject.RetweetCount = tweetJson["favorite_count"].ToObject<int>();
                    tweetObject.FavoriteCount = tweetJson["retweet_count"].ToObject<int>();
                    tweetObject.UserScreenname = tweetJson["user"]["screen_name"].ToObject<string>();

                    var tweetDate = tweetJson["created_at"].ToObject<string>();

                    DateTime tweetDateAsDate;
                    if (DateTime.TryParseExact(tweetDate, "ddd MMM dd HH:mm:ss zzz yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tweetDateAsDate))
                    {
                        tweetObject.CreatedAt = tweetDateAsDate;
                    }

                    tweetObject.RawContent = rawJson;
                    resultForThisHandle.Add(tweetObject);
                }

            }

            return resultForThisHandle;

        }

    }
}