using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Sloader.Result
{
    public abstract class BaseResult
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public abstract KnownResultType ResultType { get; }
    }
}