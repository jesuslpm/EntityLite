using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace inercya.System.Text.Json.Converters
{
    public class RoundDateJsonConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var date = reader.GetDateTime();
            if (date.Kind == DateTimeKind.Utc)
            {
                date = date.ToLocalTime();
            }
            if (date.Kind == DateTimeKind.Local)
            {
                date = new DateTime(date.Ticks, DateTimeKind.Unspecified);
            }
            return date.AddHours(12).Date;
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            if (value.Kind == DateTimeKind.Utc)
            {
                value = value.ToLocalTime();
            }
            writer.WriteStringValue(value.AddHours(12).Date.ToString("yyyy-MM-dd"));
        }
    }
}
