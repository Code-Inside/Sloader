using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using WorldDomination.Net.Http;

namespace Sloader.Engine.Crawler.Feed
{
    public class FacebookShareCountLoader : IFacebookShareCountLoader
    {
        private readonly HttpClient _httpClient;

        public FacebookShareCountLoader()
        {
            _httpClient = new HttpClient();
        }

        public FacebookShareCountLoader(FakeHttpMessageHandler messageHandler)
        {
            _httpClient = new HttpClient(messageHandler);
        }

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