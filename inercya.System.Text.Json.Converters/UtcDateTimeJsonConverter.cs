using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace inercya.System.Text.Json.Converters
{
    public class UtcDateTimeJsonConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var date = reader.GetDateTime();
            switch (date.Kind)
            {
                case DateTimeKind.Unspecified:
                    return new DateTime(date.Ticks, DateTimeKind.Utc);
                case DateTimeKind.Local:
                    return date.ToUniversalTime();
                case DateTimeKind.Utc:
                    return date;
                default:
                    throw new NotSupportedException($"DateTimeKind.{date.Kind} not supported");
            }
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            switch (value.Kind)
            {
                case DateTimeKind.Unspecified:
                    writer.WriteStringValue(new DateTime(value.Ticks, DateTimeKind.Utc));
                    break;
                case DateTimeKind.Local:
                    writer.WriteStringValue(value.ToUniversalTime());
                    break;
                case DateTimeKind.Utc:
                    writer.WriteStringValue(value);
                    break;
                default:
                    throw new NotSupportedException($"DateTimeKind.{value.Kind} not supported");
            }
        }
    }
}
