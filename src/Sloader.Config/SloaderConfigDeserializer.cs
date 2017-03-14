using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Sloader.Config
{
    /// <summary>
    /// Searches in the given ymlString for $$PLACEHOLDER$$ and replace it with a given PLACEHOLDER secret value.
    /// <para>The "full" ymlString is then deserialized to the actual SloaderConfig.</para>
    /// </summary>
    public static class SloaderConfigDeserializer
    {
        public static SloaderConfig GetConfigWithEmbeddedSecrets(string ymlString, Dictionary<string, string> secrets)
        {
            Dictionary<string, string> compareDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var secret in secrets)
            {
                compareDictionary.Add(secret.Key, secret.Value);
            }

            Regex searchForSecrets = new Regex(@"\$\$(.*?)\$\$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            var regexResult = searchForSecrets.Matches(ymlString);

            foreach (Match singleRegExResult in regexResult)
            {
                var foundPossibleSecret = singleRegExResult.Value.Replace("$$", string.Empty);

                if (compareDictionary.ContainsKey(foundPossibleSecret))
                {
                    ymlString = ymlString.Replace(singleRegExResult.Value, compareDictionary[foundPossibleSecret]);
                }
            }

            var deserializer = Constants.SloaderYamlDeserializer;
            var config = deserializer.Deserialize<SloaderConfig>(new StringReader(ymlString));

            return config;

        }
    }
}