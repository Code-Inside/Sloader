using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using WorldDomination.Net.Http;

namespace Sloader.Crawler.Feed
{
    public class FacebookShareCountLoader : IFacebookShareCountLoader
    {
        private readonly HttpMessageHandler _messageHandler;

        public FacebookShareCountLoader()
        {
               _messageHandler = new HttpClientHandler();
        }

        public FacebookShareCountLoader(HttpMessageHandler messageHandler)
        {
            _messageHandler = messageHandler;
        }

        public async Task<int> GetAsync(string url)
        {
            string facebookUrl = "https://graph.facebook.com/?id=" + url;
            string facebookContent;
            using (var httpClient = HttpClientFactory.GetHttpClient(_messageHandler))
            {
                // facebookContent sample:
                // {"id":"http://...url...","shares":1} or just
                // {"id":"http://...url..."}
                var response = await httpClient.GetAsync(facebookUrl);
                if (response.IsSuccessStatusCode)
                {
                    facebookContent = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    Trace.TraceWarning("Could not load FacebookShares for URL: " + facebookUrl);
                    return 0;
                }
            }
            var jFacebookToken = JToken.Parse(facebookContent);
            var facebookCounter = jFacebookToken.SelectToken("shares");

            if (facebookCounter == null)
                return 0;

            return facebookCounter.Value<int>();
        }
    }
}