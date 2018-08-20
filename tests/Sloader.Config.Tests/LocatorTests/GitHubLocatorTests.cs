using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Sloader.Config.Tests;
using WorldDomination.Net.Http;
using System.Collections.Generic;

namespace Sloader.Config.Tests.LocatorTests
{
    public class GitHubLocatorTests
    {
        private static string GetTestFileContent()
        {
            string responseData = TestHelperForCurrentProject.GetTestFileContent($"LocatorTests/GitHubLocator.json");

            return responseData;
        }

        private static async Task<IEnumerable<string>> InvokeSut(string owner, string repo, string path, string pattern)
        {
            
            var messageResponse = FakeHttpMessageHandler.GetStringHttpResponseMessage(GetTestFileContent());

            var fakeMessageHandler = new FakeHttpMessageHandler(new HttpMessageOptions() { HttpResponseMessage = messageResponse, RequestUri = new Uri($"https://api.github.com/repos/{owner}/{repo}/contents/{path}") });

            var sut = new SloaderConfigLocator(fakeMessageHandler);

            var result = await sut.FromGitHub("Code-Inside", "KnowYourStack", "_data", pattern);
            return result;
        }

        [Fact]
        public async Task Locator_Finds_All_Valid_Pattern_Matches()
        {
            var result = await InvokeSut("Code-Inside", "KnowYourStack", "_data", "*.Sloader.yml");

            Assert.Equal(2, result.Count());
            Assert.Contains("https://raw.githubusercontent.com/Code-Inside/KnowYourStack/master/_data/AspNetCore.Sloader.yml", result);
            Assert.Contains("https://raw.githubusercontent.com/Code-Inside/KnowYourStack/master/_data/NuGet.Sloader.yml", result);
        }

        [Fact]
        public async Task Locator_Returns_Empty_List_When_Pattern_Fails()
        {
            var result = await InvokeSut("Code-Inside", "KnowYourStack", "_data", "*.XSloader.yml");

            Assert.Empty(result);
        }
    }
}