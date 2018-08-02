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
using Sloader.Engine.Crawler.DependencyServices;
using Sloader.Engine.Util;
using Sloader.Result.Types;
using WorldDomination.Net.Http;

namespace Sloader.Engine.Crawler.Twitter
{
    /// <inheritdoc />
    /// <summary>
    /// Crawler for the Twitter Timeline API:
    /// <para>https://dev.twitter.com/rest/reference/get/statuses/user_timeline</para>
    /// </summary>
    public class TwitterTimelineCrawler : ICrawler<TwitterTimelineResult, TwitterTimelineCrawlerConfig>
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Ctor with default dependencies
        /// </summary>
        public TwitterTimelineCrawler()
        {
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Ctor for testing
        /// </summary>
        /// <param name="messageHandler">HttpMessageHandler to simulate any HTTP response</param>
        public TwitterTimelineCrawler(FakeHttpMessageHandler messageHandler)
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
        /// Actual worker method which will load the timelines for the given users.
        /// </summary>
        /// <param name="config">Twitter Users etc.</param>
        /// <returns>result for given config data</returns>
        public async Task<TwitterTimelineResult> DoWorkAsync(TwitterTimelineCrawlerConfig config)
        {
            if (string.IsNullOrWhiteSpace(config.Handle))
                return new TwitterTimelineResult();

            Trace.TraceInformation($"{nameof(TwitterTimelineCrawler)} loading stuff for '{config.Handle}'");

            var result = new TwitterTimelineResult {Tweets = new List<TwitterTimelineResult.Tweet>()};


            var handleArrayForMultipleHandles = config.Handle.Split(';');

            foreach (var handleArrayForMultipleHandle in handleArrayForMultipleHandles)
            {
                var twitterResult = await GetTwitterTimeline(OAuthToken, handleArrayForMultipleHandle.Trim(), config.IncludeRetweets, config.IncludeRawContent);
                result.Tweets.AddRange(new List<TwitterTimelineResult.Tweet>(twitterResult));
            }

            result.Tweets = result.Tweets.OrderByDescending(x => x.CreatedAt).ToList();
         
            return result;
        }

        private async Task<List<TwitterTimelineResult.Tweet>> GetTwitterTimeline(string oauthToken, string screenname, bool includeRetweets, bool includeRawContent)
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

                    TwitterTimelineResult.Tweet tweetObject =
                        new TwitterTimelineResult.Tweet
                        {
                            Id = tweet["id_str"].ToObject<string>(),
                            Text = tweet["text"].ToObject<string>().ToCleanString(),
                            Source = tweet["source"].ToObject<string>(),
                            RetweetCount = tweet["favorite_count"].ToObject<int>(),
                            FavoriteCount = tweet["retweet_count"].ToObject<int>(),
                            UserScreenname = tweet["user"]["screen_name"].ToObject<string>()
                        };

                    var tweetDate = tweet["created_at"].ToObject<string>();

                    if (DateTime.TryParseExact(tweetDate, "ddd MMM dd HH:mm:ss zzz yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var tweetDateAsDate))
                    {
                        tweetObject.CreatedAt = tweetDateAsDate;
                    }

                    if(includeRawContent)
                    {
                        tweetObject.RawContent = rawJson;
                    }

                    resultForThisHandle.Add(tweetObject);
                }

            }

            return resultForThisHandle;

        }

    }
}