using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using WorldDomination.Net.Http;

namespace Sloader.Bootstrapper.Crawler.Feed
{
    public class FacebookShareCountLoader : IFacebookShareCountLoader
    {
        public async Task<int> GetAsync(string url)
        {
            string facebookUrl = "https://graph.facebook.com/?id=" + url;
            string facebookContent;
            using (var httpClient = HttpClientFactory.GetHttpClient())
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

            if (facebookCounter == null || facebookCounter.HasValues == false)
                return 0;

            return facebookCounter.Value<int>();
        }
    }
}