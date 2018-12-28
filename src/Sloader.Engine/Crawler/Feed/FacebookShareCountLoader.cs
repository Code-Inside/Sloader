using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using WorldDomination.Net.Http;

namespace Sloader.Engine.Crawler.Feed
{
    /// <inheritdoc />
    /// <summary>
    /// Should return for a given url the like count on facebook
    /// </summary>
    public class FacebookShareCountLoader : IFacebookShareCountLoader
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Ctor, creates a HttpClient
        /// </summary>
        public FacebookShareCountLoader()
        {
            _httpClient = SloaderRunner.HttpClient; 
        }

        /// <summary>
        /// Ctor for testing
        /// </summary>
        /// <param name="messageHandler">HttpMessageHandler to simulate any HTTP response</param>
        public FacebookShareCountLoader(FakeHttpMessageHandler messageHandler)
        {
            _httpClient = new HttpClient(messageHandler);
        }

        /// <summary>
        /// Uses the https://graph.facebook.com/?id=" + url; from facebook to fetch the like count for a given url.
        /// </summary>
        /// <param name="url">Given url, e.g. a blogpost url etc.</param>
        /// <returns>Like-count as task</returns>
        public async Task<int> GetAsync(string url)
        {
            string facebookUrl = "https://graph.facebook.com/?id=" + url;
            string facebookContent;
            // facebookContent sample:
            // {"id":"http://...url...","shares":1} or just
            // {"id":"http://...url..."}
            var response = await _httpClient.GetAsync(facebookUrl);
            if (response.IsSuccessStatusCode)
            {
                facebookContent = await response.Content.ReadAsStringAsync();
            }
            else
            {
                Trace.TraceWarning("Could not load FacebookShares for URL: " + facebookUrl);
                return 0;
            }

            var jFacebookToken = JToken.Parse(facebookContent);
            var facebookCounter = jFacebookToken.SelectToken("shares");

            if (facebookCounter == null)
                return 0;

            return facebookCounter.Value<int>();
        }
    }
}