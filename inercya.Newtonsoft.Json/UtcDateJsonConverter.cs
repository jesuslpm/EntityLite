using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace inercya.Newtonsoft.Json.Converters
{
    public class UtcDateJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(DateTime) == objectType || typeof(DateTime) == objectType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            DateTime? result = null;
            if (reader.TokenType == JsonToken.String)
            {
                string valueAsString = (string)reader.Value;
                DateTime? date = (DateTime?)JToken.Parse("\"" + valueAsString + "\"");
                if (date.HasValue) result = date;
            }
            else if (reader.TokenType == JsonToken.Date)
            {
                result = (DateTime)reader.Value;
            }
            if (result == null && objectType == typeof(DateTime))
            {
                result = DateTime.MinValue;
            }
            if (result.HasValue)
            {
                if (result.Value.Kind == DateTimeKind.Unspecified)
                {
                    result = new DateTime(result.Value.Ticks, DateTimeKind.Utc);
                }
                else if (result.Value.Kind == DateTimeKind.Local)
                {
                    result = result.Value.ToUniversalTime();
                }
            }
            return result;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                DateTime date = (DateTime)value;
                if (date.Kind == DateTimeKind.Unspecified)
                {
                    date = new DateTime(date.Ticks, DateTimeKind.Utc);
                }
                else if (date.Kind == DateTimeKind.Local)
                {
                    date = date.ToUniversalTime();
                }
                writer.WriteValue(date);
            }
        }
    }
}
