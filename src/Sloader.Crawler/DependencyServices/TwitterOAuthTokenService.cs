using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using WorldDomination.Net.Http;

namespace Sloader.Crawler.DependencyServices
{
    public class TwitterOAuthTokenService : ITwitterOAuthTokenService, IDisposable
    {
        private readonly HttpMessageHandler _messageHandler;

        public TwitterOAuthTokenService()
        {
               _messageHandler = new HttpClientHandler();
        }

        public TwitterOAuthTokenService(HttpMessageHandler messageHandler)
        {
            _messageHandler = messageHandler;
        }

        public async Task<string> GetAsync(string consumerKey, string consumerSecret)
        {
            string oauthToken;

            using (var httpClient = HttpClientFactory.GetHttpClient(_messageHandler, false))
            {
                var authHeaderParameter = Convert.ToBase64String(Encoding.UTF8.GetBytes(Uri.EscapeDataString(consumerKey) + ":" +
                                                                                        Uri.EscapeDataString((consumerSecret))));

                const string postBody = "grant_type=client_credentials";

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderParameter);

                var response = await httpClient.PostAsync("https://api.twitter.com/oauth2/token", new StringContent(postBody, Encoding.UTF8, "application/x-www-form-urlencoded"));

                if (response.IsSuccessStatusCode)
                {

                    string oauthtoken = await response.Content.ReadAsStringAsync();
                    var jToken = JToken.Parse(oauthtoken);
                    var accessToken = jToken.SelectToken("access_token");

                    oauthToken = accessToken.Value<string>();
                }
                else
                {
                    Trace.TraceWarning("OAuth Token could not be retrieved.");
                    oauthToken = string.Empty;
                }
            }

            return oauthToken;
        }

        public void Dispose()
        {
            _messageHandler.Dispose();
        }
    }
}