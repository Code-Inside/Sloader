using System;
using System.IO;
using System.Reflection;

namespace Sloader.Engine.Tests
{
    /// <summary>
    /// Wrapper around basic File IO Operations for the TestFiles.
    /// I re-introduced this helper to have one centralized way to access 
    /// sample data. Sample data could be embedded as Resource or just pure File 
    /// Stuff. 
    /// </summary>
    public static class TestHelperForCurrentProject
    {
        private const string ResourcePath = "Sloader.Engine.Tests.{0}";

        public static string GetTestFileContent(string path)
        {
            return File.ReadAllText(path);
        }
        public static string GetTestFilePath(params string[] strings)
        {
            return Path.Combine(strings);
        }

        public static Stream GetTestResourceFileStream(string folderAndFileInProjectPath)
        {
            var asm = Assembly.GetExecutingAssembly();
            var resource = string.Format(ResourcePath, folderAndFileInProjectPath);

            return asm.GetManifestResourceStream(resource);
        }

        public static string GetTestResourceFileContent(string folderAndFileInProjectPath)
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
