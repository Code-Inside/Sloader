using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using WorldDomination.Net.Http;

namespace Sloader.Config
{
    /// <summary>
    /// Helper class to load sloader config paths from different locations
    /// </summary>
    public class SloaderConfigLocator
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Ctor with default dependencies
        /// </summary>
        public SloaderConfigLocator()
        {
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Ctor for testing
        /// </summary>
        /// <param name="messageHandler">HttpMessageHandler to simulate any HTTP response</param>
        public SloaderConfigLocator(FakeHttpMessageHandler messageHandler)
        {
            _httpClient = new HttpClient(messageHandler);
        }

        /// <summary>
        /// Helper method to load sloader configs from a given repo with a given pattern
        /// </summary>
        /// <param name="owner">GitHub Owner e.g. Code-Inside</param>
        /// <param name="repo">GitHub Repository e.g. Code-Inside</param>
        /// <param name="path">Folder path e.g. _data or folderY/folderX </param>
        /// <param name="sloaderPattern">File pattern - currently pattern must start with a '*' like *.Sloader.yml</param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> FromGitHub(string owner, string repo, string path, string sloaderPattern)
        {
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Anything");

            string apiCall = $"https://api.github.com/repos/{owner}/{repo}/contents/{path}";

            Trace.TraceInformation($"{nameof(SloaderConfigLocator)} {nameof(FromGitHub)} invoked for '{apiCall}'.");

            var response = await _httpClient.GetAsync(apiCall);

            response.EnsureSuccessStatusCode();

            string githubResult = await response.Content.ReadAsStringAsync();

            var folderContent = JsonConvert.DeserializeObject<JArray>(githubResult);

            List<string> githubUrls = new List<string>();
            foreach (var content in folderContent)
            {
                var isFile = content["type"].ToObject<string>();

                if (isFile.ToLowerInvariant() == "file")
                {
                    var fileName = content["name"].ToObject<string>();
                    if (sloaderPattern.StartsWith("*"))
                    {
                        var cleanPattern = sloaderPattern.Replace("*", "").ToLowerInvariant();
                        if (fileName.ToLowerInvariant().EndsWith(cleanPattern))
                        {
                            var rawUrl = content["download_url"].ToObject<string>();
                            githubUrls.Add(rawUrl);
                        }
                    }

                }
            }

            Trace.TraceInformation($"{nameof(SloaderConfigLocator)} {nameof(FromGitHub)} found '{githubUrls.Count}' File-URLs.");

            return githubUrls;
        }
    }
}