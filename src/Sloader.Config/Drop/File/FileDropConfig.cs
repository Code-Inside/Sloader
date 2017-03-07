namespace Sloader.Config.Drop.File
{
    /// <summary>
    /// Drops the output as a File.
    /// </summary>
    /// <example>
    /// Demo yml config:
    /// <code>
    /// FileDrops:
    /// - FilePath: "test.json"
    /// </code>
    /// </example>
    public class FileDropConfig 
    {
        /// <summary>
        /// Relative or absolute path, e.g. test.json or D:/Temp/test.json
        /// </summary>
        public string FilePath { get; set; }
    }
}