using System;
using System.IO;
using System.Reflection;

namespace Sloader.Tests
{
    public static class TestHelperForCurrentProject
    {
        private const string ResourcePath = "Sloader.Tests.{0}";

        public static string GetTestFileContent(string folderAndFileInProjectPath)
        {
            var asm = Assembly.GetExecutingAssembly();
            var resource = string.Format(ResourcePath, folderAndFileInProjectPath);

            using (var stream = asm.GetManifestResourceStream(resource))
            {
                if (stream != null)
                {
                    var reader = new StreamReader(stream);
                    return reader.ReadToEnd();
                }
            }
            return String.Empty;
        }

    }
}