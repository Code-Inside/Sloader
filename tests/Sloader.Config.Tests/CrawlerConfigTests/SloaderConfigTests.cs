using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Sloader.Config.Tests.CrawlerConfigTests
{
    public class SloaderConfigTests
    {
        [Fact]
        public void Deserialize_Of_Testdata_To_Config_Works()
        {
            string yaml = TestHelperForCurrentProject.GetTestFileContent("CrawlerConfigTests/Sample.yaml");

            var deserializer = Constants.SloaderYamlDeserializer;
            var result = deserializer.Deserialize<SloaderConfig>(new StringReader(yaml));

            Assert.NotNull(result.Crawler);
            Assert.Equal(3, result.Crawler.FeedsToCrawl.Count);
            Assert.Equal(3, result.Crawler.TwitterTimelinesToCrawl.Count);
        }

        [Fact]
        public void Deserialize_Of_Secrets_In_Testdata_To_Config_Works()
        {
            string yaml = TestHelperForCurrentProject.GetTestFileContent("CrawlerConfigTests/Sample.yaml");

            var deserializer = Constants.SloaderYamlDeserializer;
            var result = deserializer.Deserialize<SloaderConfig>(new StringReader(yaml));

            Assert.NotNull(result.Secrets);
            Assert.Equal("HelloWorldKey", result.Secrets.TwitterConsumerKey);
            Assert.Equal("HelloWorldSecret", result.Secrets.TwitterConsumerSecret);
        }

        [Fact]
        public void Secrets_Is_Never_Null()
        {
            StringBuilder fakeYaml = new StringBuilder();
            fakeYaml.AppendLine("Crawler:");
            fakeYaml.AppendLine("  TwitterTimelinesToCrawl:");
            fakeYaml.AppendLine("  - Handle: codeinsideblog");

            var deserializer = Constants.SloaderYamlDeserializer;
            var result = deserializer.Deserialize<SloaderConfig>(new StringReader(fakeYaml.ToString()));

            Assert.NotNull(result.Secrets);
        }

        [Fact]
        public void Parser_Can_Embed_Secrets()
        {
            StringBuilder fakeYaml = new StringBuilder();
            fakeYaml.AppendLine("Secrets:");
            fakeYaml.AppendLine("  TwitterConsumerKey: $$SecretPlaceholder1$$");
            fakeYaml.AppendLine("  TwitterConsumerSecret: $$SecretPlaceholder2$$");
            fakeYaml.AppendLine("");
            fakeYaml.AppendLine("Crawler:");
            fakeYaml.AppendLine("  TwitterTimelinesToCrawl:");
            fakeYaml.AppendLine("  - Handle: codeinsideblog");

            Dictionary<string, string> secrets = new Dictionary<string, string>();
            secrets.Add("SecretPlaceholder1", "This is the secret key");
            secrets.Add("SecretPlaceholder2", "This is the secret secret");

            var result = SloaderConfigLoader.Parse(fakeYaml.ToString(), secrets);

            Assert.Equal("This is the secret key", result.Secrets.TwitterConsumerKey);
            Assert.Equal("This is the secret secret", result.Secrets.TwitterConsumerSecret);
        }

        [Fact]
        public void Deserializer_Can_Embed_Secrets()
        {
            StringBuilder fakeYaml = new StringBuilder();
            fakeYaml.AppendLine("Secrets:");
            fakeYaml.AppendLine("  TwitterConsumerKey: $$SecretPlaceholder1$$");
            fakeYaml.AppendLine("  TwitterConsumerSecret: $$SecretPlaceholder2$$");
            fakeYaml.AppendLine("");
            fakeYaml.AppendLine("Crawler:");
            fakeYaml.AppendLine("  TwitterTimelinesToCrawl:");
            fakeYaml.AppendLine("  - Handle: codeinsideblog");

            Dictionary<string, string> secrets = new Dictionary<string, string>();
            secrets.Add("SecretPlaceholder1", "This is the secret key");
            secrets.Add("SecretPlaceholder2", "This is the secret secret");

            var result = SloaderConfigDeserializer.GetConfigWithEmbeddedSecrets(fakeYaml.ToString(), secrets);

            Assert.Equal("This is the secret key", result.Secrets.TwitterConsumerKey);
            Assert.Equal("This is the secret secret", result.Secrets.TwitterConsumerSecret);
        }

        [Fact]
        public void Deserializer_Can_Embed_Secrets_Case_Insensitive()
        {
            StringBuilder fakeYaml = new StringBuilder();
            fakeYaml.AppendLine("Secrets:");
            fakeYaml.AppendLine("  TwitterConsumerKey: $$SecretPlaceholder1$$");
            fakeYaml.AppendLine("  TwitterConsumerSecret: $$secretplaceholder2$$");
            fakeYaml.AppendLine("");
            fakeYaml.AppendLine("Crawler:");
            fakeYaml.AppendLine("  TwitterTimelinesToCrawl:");
            fakeYaml.AppendLine("  - Handle: codeinsideblog");

            Dictionary<string, string> secrets = new Dictionary<string, string>();
            secrets.Add("SECRETPlaceholder1", "This is the secret key");
            secrets.Add("SecretPlaceholder2", "This is the secret secret");

            var result = SloaderConfigDeserializer.GetConfigWithEmbeddedSecrets(fakeYaml.ToString(), secrets);

            Assert.Equal("This is the secret key", result.Secrets.TwitterConsumerKey);
            Assert.Equal("This is the secret secret", result.Secrets.TwitterConsumerSecret);
        }

    }
}
