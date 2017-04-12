namespace Sloader.Result
{
    /// <summary>
    /// Interface to signal that the actual API would return more data.
    /// <para>The property "RawData" will include the actual API result and not the Sloader normalized.</para>
    /// <para>Use with caution, because this is a "raw" data store</para>
    /// </summary>
    public interface IHaveRawContent
    {
        /// <summary>
        /// JSON Serialized raw content
        /// </summary>
        string RawContent { get; set; }
    }
}