using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace inercya.Newtonsoft.Json.Converters
{
    public class RoundDateJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(DateTime) == objectType || typeof(DateTime) == objectType;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            DateTime? result = null;
            if (reader.TokenType == JsonToken.String)
            {
                string valueAsString = (string)reader.Value;
                DateTime? date = (DateTime?)JToken.Parse("\"" + valueAsString + "\"");
                if (date.HasValue) result = date.Value.AddHours(12).Date;
            }
            else if (reader.TokenType == JsonToken.Date)
            {
                result = ((DateTime)reader.Value).AddHours(12).Date;
            }
            if (result == null && objectType == typeof(DateTime)) return DateTime.MinValue;
            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                DateTime date = ((DateTime)value).AddHours(12).Date;
                writer.WriteValue(date.ToString("yyyy-MM-dd"));
            }
        }
    }
}
