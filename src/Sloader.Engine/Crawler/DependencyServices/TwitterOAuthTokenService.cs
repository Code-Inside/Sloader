using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using WorldDomination.Net.Http;

namespace Sloader.Engine.Crawler.DependencyServices
{
    /// <inheritdoc />
    /// <summary>
    /// Implementation for the OAuthTokenService
    /// </summary>
    public class TwitterOAuthTokenService : ITwitterOAuthTokenService
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Ctor, will create a HttpClient
        /// </summary>
        public TwitterOAuthTokenService()
        {
            _httpClient = SloaderRunner.HttpClient;
        }

        /// <summary>
        /// Ctor for testing
        /// </summary>
        /// <param name="messageHandler">A testing/fake message handler might be inserted here.</param>
        public TwitterOAuthTokenService(FakeHttpMessageHandler messageHandler)
        {
            _httpClient = new HttpClient(messageHandler);
        }

        /// <summary>
        /// Uses the key/secret to get the OAuthToken via the HttpClient.
        /// </summary>
        /// <param name="consumerKey">Twitter API Consumer Key</param>
        /// <param name="consumerSecret">Twitter API Consumer Secret</param>
        /// <returns>Returns the OAuth token as task</returns>
        public async Task<string> GetAsync(string consumerKey, string consumerSecret)
        {
            string oauthToken;

            var authHeaderParameter = Convert.ToBase64String(Encoding.UTF8.GetBytes(Uri.EscapeDataString(consumerKey) + ":" +
                                                                                    Uri.EscapeDataString((consumerSecret))));

            const string postBody = "grant_type=client_credentials";

            HttpResponseMessage response;

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api.twitter.com/oauth2/token"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeaderParameter);
                requestMessage.Content = new StringContent(postBody, Encoding.UTF8, "application/x-www-form-urlencoded");
                response = await _httpClient.SendAsync(requestMessage);
            }

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

            return oauthToken;
        }
    }
}