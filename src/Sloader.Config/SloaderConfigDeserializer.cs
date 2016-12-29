using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Sloader.Config
{
    public static class SloaderConfigDeserializer
    {
        public static SloaderConfig GetConfigWithEmbeddedSecrets(string yamlString, Dictionary<string, string> secrets)
        {
            Dictionary<string, string> compareDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var secret in secrets)
            {
                compareDictionary.Add(secret.Key, secret.Value);
            }

            Regex searchForSecrets = new Regex(@"\$\$(.*?)\$\$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            var regexResult = searchForSecrets.Matches(yamlString);

            foreach (Match singleRegExResult in regexResult)
            {
                var foundPossibleSecret = singleRegExResult.Value.Replace("$$", string.Empty);

                if (compareDictionary.ContainsKey(foundPossibleSecret))
                {
                    yamlString = yamlString.Replace(singleRegExResult.Value, compareDictionary[foundPossibleSecret]);
                }
            }

            var deserializer = Constants.SloaderYamlDeserializer;
            var config = deserializer.Deserialize<SloaderConfig>(new StringReader(yamlString));

            return config;

        }
    }
}