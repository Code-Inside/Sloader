using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sloader.Result.Types;

namespace Sloader.Result
{
    public class ResultConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(BaseResult));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            var resultType = jo["ResultType"].Value<string>();

            KnownResultType knownType;
            if (Enum.TryParse(resultType, true, out knownType))
            {
                if (knownType == KnownResultType.Feed)
                    return jo.ToObject<FeedResult>(serializer);

                if (knownType == KnownResultType.TwitterTimeline)
                    return jo.ToObject<TwitterTimelineResult>(serializer);

            }

            
            return null;
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}