using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Octokit;
using Sloader.Config;
using Sloader.Config.Drop.GitHub;
using Sloader.Result;
using System;

namespace Sloader.Engine.Drop.GitHub
{
    /// <inheritdoc />
    /// <summary>
    /// Will drop the results on GitHub as one file.
    /// </summary>
    public class GitHubDrop : IDrop<GitHubDropConfig>
    {
        /// <summary>
        /// Needed token to access the GitHub Api
        /// </summary>
        /// <see cref="SloaderSecrets.GitHubAccessToken"/>
        public string AccessToken { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Actual work method - will write everything to GitHub via Octokit.net.
        /// </summary>
        /// <param name="config">Desired repo/org/file path etc.</param>
        /// <param name="crawlerRun">Drop content.</param>
        /// <returns></returns>
        public async Task DoWorkAsync(GitHubDropConfig config, CrawlerRun crawlerRun)
        {
            Trace.TraceInformation($"{nameof(GitHubDrop)} dropping stuff for owner '{config.Owner}' on '{config.Repo}':'{config.Branch}' for '{config.FilePath}' ");

	        if (string.IsNullOrWhiteSpace(AccessToken))
	        {
		        throw new ArgumentException($"{nameof(AccessToken)} is not applied for {nameof(GitHubDrop)} work action.");
	        }

            var ghClient =
                new GitHubClient(new ProductHeaderValue("Sloader")) {Credentials = new Credentials(AccessToken)};

            // github variables
            var owner = config.Owner;
            var repo = config.Repo;
            var branch = config.Branch;

            var targetFile = config.FilePath;

            var content = crawlerRun.ToJson();

            try
            {
                // try to get the file (and with the file the last commit sha)
                var existingFile = await ghClient.Repository.Content.GetAllContentsByRef(owner, repo, targetFile, branch);

                // update the file
                await ghClient.Repository.Content.UpdateFile(owner, repo, targetFile,
                   new UpdateFileRequest($"Sloader update on {targetFile}", content, existingFile.First().Sha, branch));
            }
            catch (NotFoundException)
            {
                // if file is not found, create it
                try
                {
                    await ghClient.Repository.Content.CreateFile(owner, repo, targetFile, new CreateFileRequest($"Sloader create for {targetFile}", content, branch));
                }
                catch (Exception exc)
                {
                    Trace.TraceError($"{nameof(GitHubDrop)} failed with '{exc.Message}' on '{config.Repo}':'{config.Branch}' for '{config.FilePath}'. Make sure your account has write access!");
                    throw;
                }
            }
        }
    }
}
