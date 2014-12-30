using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using WorldDomination.Net.Http;

namespace Sloader.Bootstrapper.Crawler.Feed
{
    public class TwitterTweetCountLoader : ITwitterTweetCountLoader
    {
        public async Task<int> GetAsync(string url)
        {
            string twitterUrl = "http://urls.api.twitter.com/1/urls/count.json?url=" + url;
            string twitterContent;
            using (var httpClient = HttpClientFactory.GetHttpClient())
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

            if (twitterCounter == null || twitterCounter.HasValues == false)
                return 0;

            return twitterCounter.Value<int>();
        }
    }
}