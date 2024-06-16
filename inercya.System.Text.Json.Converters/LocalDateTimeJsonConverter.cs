using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace inercya.System.Text.Json.Converters
{
    public class LocalDateTimeJsonConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var date = reader.GetDateTime();
            switch (date.Kind)
            {
                case DateTimeKind.Unspecified:
                    return new DateTime(date.Ticks, DateTimeKind.Local);
                case DateTimeKind.Local:
                    return date;
                case DateTimeKind.Utc:
                    return date.ToLocalTime();
                default:
                    throw new NotSupportedException($"DateTimeKind.{date.Kind} not supported");
            }
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            switch (value.Kind)
            {
                case DateTimeKind.Unspecified:
                    writer.WriteStringValue(new DateTime(value.Ticks, DateTimeKind.Local));
                    break;
                case DateTimeKind.Local:
                    writer.WriteStringValue(value);
                    break;
                case DateTimeKind.Utc:
                    writer.WriteStringValue(value.ToLocalTime());
                    break;
                default:
                    throw new NotSupportedException($"DateTimeKind.{value.Kind} not supported");
            }
        }
    }
}
