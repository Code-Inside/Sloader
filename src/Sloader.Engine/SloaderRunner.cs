using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using Sloader.Config;
using System.Linq;
using Sloader.Engine.Crawler.Feed;
using Sloader.Engine.Crawler.GitHub;
using Sloader.Engine.Drop.File;
using Sloader.Engine.Drop.GitHub;
using Sloader.Result;
using System.IO;
using System.Net.Http;

namespace Sloader.Engine
{
	/// <summary>
	/// The main actor in Sloader:
	/// <para>Loads the Sloader.Config.</para>
	/// <para>Run through all crawlers and drops.</para>
	/// </summary>
	public class SloaderRunner
	{
        public static HttpClient HttpClient = new HttpClient();

        /// <summary>
        /// AutoRun does most of the stuff convention based and will load the config from a local Sloader.yml in the same directory
        /// or for a file defined via the SloaderConfigPath.
        /// <para>The AutoRun will also scan all AppSettings and try to fill up all missing Secrets.</para>
        /// </summary>
        /// <see cref="FixedConfigKeys.SloaderConfigPath"/>
        public static async Task AutoRun()
		{
			Trace.TraceInformation($"AutoRun invoked.");

            var localSloaderFile = Directory.GetFiles(Directory.GetCurrentDirectory(), "Sloader.yml").FirstOrDefault();

            if(string.IsNullOrWhiteSpace(localSloaderFile) == false)
            {
                await AutoRunInternal(localSloaderFile, GetSecrectsFromConfigOrEnvironment());
            }
            else
            {
                var sloaderPathFromEnv = Environment.GetEnvironmentVariable(FixedConfigKeys.SloaderConfigPath);
                var sloaderPathFromConfig = ConfigurationManager.AppSettings[FixedConfigKeys.SloaderConfigPath];
                if (string.IsNullOrWhiteSpace(sloaderPathFromEnv) == false || string.IsNullOrWhiteSpace(sloaderPathFromConfig) == false)
                {
                    string sloaderPath;
                    if (string.IsNullOrWhiteSpace(sloaderPathFromEnv) == false)
                    {
                        sloaderPath = sloaderPathFromEnv;
                        Trace.TraceError($"{FixedConfigKeys.SloaderConfigPath} Key with path to Sloader.yml found in environment: '{sloaderPathFromEnv}'.");
                    }
                    else
                    {
                        sloaderPath = sloaderPathFromConfig;
                        Trace.TraceError($"{FixedConfigKeys.SloaderConfigPath} Key with path to Sloader.yml found in appsettings config: '{sloaderPathFromConfig}'.");
                    }

                    await AutoRunInternal(sloaderPath, GetSecrectsFromConfigOrEnvironment());
                }
                else
                {
                    Trace.TraceError($"Key with path to Sloader.yml missing in AppSettings or Environment: '{FixedConfigKeys.SloaderConfigPath}'");
                }
            }
			
		}

		/// <summary>
		/// Same as the parameterless "AutoRun", but you need to inject the needed SloaderConfigPath.
		/// <para>Like the parameterless AutoRun this method will also scan all AppSettings and try to fill up all missing Secrets.</para>
		/// </summary>
		/// <param name="sloaderConfigPath">Relative or absolute file path or URL to a valid Sloader config.</param>
		public static async Task AutoRun(string sloaderConfigPath)
		{
			Trace.TraceInformation($"AutoRun invoked with {nameof(sloaderConfigPath)}: '{sloaderConfigPath}'.");

			var settingsDictionary = GetSecrectsFromConfigOrEnvironment();

			await AutoRunInternal(sloaderConfigPath, settingsDictionary);

		}

		/// <summary>
		/// Same aus the parameterless "AutoRun", but you need to inject the needed SloaderConfigPath and the Secrets yourself.
		/// </summary>
		/// <param name="sloaderConfigPath">Relative or absolute file path or URL to a valid Sloader config.</param>
		/// <param name="secrets">Secrets, which may be used as placeholders inside the Sloader config.</param>
		public static async Task AutoRun(string sloaderConfigPath, Dictionary<string, string> secrets)
		{
			Trace.TraceInformation($"AutoRun invoked with {nameof(sloaderConfigPath)}: '{sloaderConfigPath}' & '{secrets.Count}' {nameof(secrets)}.");

			await AutoRunInternal(sloaderConfigPath, secrets);
		}

		/// <summary>
		/// Internal logic for the AutoRun-methods.
		/// </summary>
		private static async Task AutoRunInternal(string sloaderConfigPath, Dictionary<string, string> secrets)
		{
			var config =
				 await
					 SloaderConfig.Load(sloaderConfigPath, secrets);

			Trace.TraceInformation($"SloaderConfig loaded - init {nameof(SloaderRunner)}");

			var runner = new SloaderRunner(config);
			var crawlerRun = await runner.RunAllCrawlers();
			await runner.RunThroughDrop(crawlerRun);
		}

		private readonly SloaderConfig _config;

		public SloaderRunner(SloaderConfig config)
		{
			_config = config;
		}

		/// <summary>
		/// Will run through all applied crawlers.
		/// </summary>
		public async Task<CrawlerRun> RunAllCrawlers()
		{
			Trace.TraceInformation($"{nameof(RunAllCrawlers)} invoked.");
			var watch = new Stopwatch();
			watch.Start();

			var crawlerRunResult = new CrawlerRun();

			if (_config.Crawler == null)
				return crawlerRunResult;

			// Feeds
			if (_config.Crawler.FeedsToCrawl.Any())
			{
				foreach (var feedConfig in _config.Crawler.FeedsToCrawl)
				{
					var feedCrawler = new FeedCrawler();
					var feedResult = await feedCrawler.DoWorkAsync(feedConfig);
					crawlerRunResult.AddResultDataPair(feedConfig.Key, feedResult);
				}

			}

			// GitHubEvents
			if (_config.Crawler.GitHubEventsToCrawl.Any())
			{
				foreach (var githubEventConfig in _config.Crawler.GitHubEventsToCrawl)
				{
					var eventCrawler = new GitHubEventCrawler();
					var eventResult = await eventCrawler.DoWorkAsync(githubEventConfig);
					crawlerRunResult.AddResultDataPair(githubEventConfig.Key, eventResult);
				}
			}

			// GitHubIssues
			if (_config.Crawler.GitHubIssuesToCrawl.Any())
			{
				foreach (var githubIssueConfig in _config.Crawler.GitHubIssuesToCrawl)
				{
					var issueCrawler = new GitHubIssueCrawler();
					var issueResult = await issueCrawler.DoWorkAsync(githubIssueConfig);
					crawlerRunResult.AddResultDataPair(githubIssueConfig.Key, issueResult);
				}
			}

			watch.Stop();
			crawlerRunResult.RunDurationInMilliseconds = watch.ElapsedMilliseconds;
			crawlerRunResult.RunOn = DateTime.UtcNow;

			Trace.TraceInformation($"{nameof(RunAllCrawlers)} done.");

			return crawlerRunResult;
		}

		/// <summary>
		/// Will run through all applied drops.
		/// </summary>
		/// <param name="crawlerRun">Will drop the CrawlerRun at the configured drops.</param>
		public async Task RunThroughDrop(CrawlerRun crawlerRun)
		{
			Trace.TraceInformation($"{nameof(RunThroughDrop)} invoked.");

			if (_config.Drop != null)
			{
				foreach (var fileDropConfig in _config.Drop.FileDrops)
				{
					var fileDrop = new FileDrop();
					await fileDrop.DoWorkAsync(fileDropConfig, crawlerRun);
				}

				foreach (var gitHubDropConfig in _config.Drop.GitHubDrops)
				{
					if (!string.IsNullOrWhiteSpace(_config.Secrets.GitHubAccessToken))
					{
						var gitHubDrop = new GitHubDrop
						{
							AccessToken = _config.Secrets.GitHubAccessToken
						};

						await gitHubDrop.DoWorkAsync(gitHubDropConfig, crawlerRun);
					}
				}
			}

			Trace.TraceInformation($"{nameof(RunThroughDrop)} done.");
		}

		/// <summary>
		/// Secrets can be applied via legacy appsettings or environment variables (e.g. for Azure Functions etc.).
		/// This method will apply all settings in a single dictionary.
		/// </summary>
		/// <returns>Dictionary with all applied settings to search for secrets.</returns>
		private static Dictionary<string, string> GetSecrectsFromConfigOrEnvironment()
		{
			Dictionary<string, string> dictionaryForSecretsAndPath = new Dictionary<string, string>();

			if (ConfigurationManager.AppSettings != null && ConfigurationManager.AppSettings.HasKeys())
			{
				dictionaryForSecretsAndPath = ConfigurationManager.AppSettings.AllKeys.ToDictionary(k => k,
					v => ConfigurationManager.AppSettings[v]);
			}

			var fromEnv = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Process);

			foreach (DictionaryEntry envVar in fromEnv)
			{
				if (dictionaryForSecretsAndPath.ContainsKey(envVar.Key.ToString()) == false)
				{
					dictionaryForSecretsAndPath.Add(envVar.Key.ToString(), envVar.Value.ToString());
				}
			}

			return dictionaryForSecretsAndPath;
		}
	}
}