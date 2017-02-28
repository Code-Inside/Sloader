using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Octokit;
using Sloader.Config.Drop.GitHub;
using Sloader.Result;

namespace Sloader.Engine.Drop.GitHub
{
    public class GitHubDrop : IDrop<GitHubDropConfig>
    {
        public string AccessToken { get; set; }

        public async Task DoWorkAsync(GitHubDropConfig config, CrawlerRun crawlerRun)
        {
            Trace.TraceInformation($"{nameof(GitHubDrop)} dropping stuff for owner '{config.Owner}' on '{config.Repo}':'{config.Branch}' for '{config.FilePath}' ");

            var ghClient = new GitHubClient(new ProductHeaderValue("Sloader"));
            ghClient.Credentials = new Credentials(AccessToken);

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
                   new UpdateFileRequest("Sloader update", content, existingFile.First().Sha, branch));
            }
            catch (Octokit.NotFoundException)
            {
                // if file is not found, create it
                await ghClient.Repository.Content.CreateFile(owner, repo, targetFile, new CreateFileRequest("Sloader create", content, branch));
            }
        }
    }
}
