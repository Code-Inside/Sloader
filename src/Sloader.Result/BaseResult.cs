using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Sloader.Result
{
    /// <summary>
    /// Base type for all results
    /// </summary>
    public abstract class BaseResult
    {
        /// <summary>
        /// This property will tell clients the exact result type.
        /// <para>Each result type has different properties, so use this to determine which property is available.</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public abstract KnownResultType ResultType { get; }
    }
}