using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using WorldDomination.Net.Http;

namespace Sloader.Crawler.Feed
{
    public class TwitterTweetCountLoader : ITwitterTweetCountLoader, IDisposable
    {
        private readonly HttpMessageHandler _messageHandler;

        public TwitterTweetCountLoader()
        {
               _messageHandler = new HttpClientHandler();
        }

        public TwitterTweetCountLoader(HttpMessageHandler messageHandler)
        {
            _messageHandler = messageHandler;
        }

        public async Task<int> GetAsync(string url)
        {
            string twitterUrl = "http://urls.api.twitter.com/1/urls/count.json?url=" + url;
            string twitterContent;
            using (var httpClient = HttpClientFactory.GetHttpClient(_messageHandler, false))
            {
                // twitterContent sample:
                // {"count":0,"url":"http://...url..."}
                var response = await httpClient.GetAsync(twitterUrl);
                if (response.IsSuccessStatusCode) {
                    twitterContent = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    Trace.TraceWarning("Could not load TwitterCounts for URL: " + twitterUrl);
                    return 0;
                }
            }
            var jTwitterToken = JToken.Parse(twitterContent);
            var twitterCounter = jTwitterToken.SelectToken("count");

            if (twitterCounter == null)
                return 0;

            return twitterCounter.Value<int>();
        }

        public void Dispose()
        {
            _messageHandler.Dispose();
        }
    }
}