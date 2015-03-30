using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sloader.Results
{
    public class CrawlerResultConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(BaseCrawlerResult));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            var resultType = jo["ResultType"].Value<string>();

            if (resultType == KnownCrawlerResultType.Feed.ToString())
                return jo.ToObject<FeedCrawlerResult>(serializer);

            if (resultType == KnownCrawlerResultType.TwitterTimeline.ToString())
                return jo.ToObject<TwitterTimelineCrawlerResult>(serializer);

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