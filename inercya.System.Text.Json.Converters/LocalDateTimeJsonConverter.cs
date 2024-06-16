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
                case DateTimeKind.Utc:
                    return date.ToLocalTime();
                case DateTimeKind.Local:
                    return date;
                case DateTimeKind.Unspecified:
                    return DateTime.SpecifyKind(date, DateTimeKind.Local);
                default:
                    throw new NotSupportedException($"DateTimeKind.{date.Kind} not supported");
            }
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            switch (value.Kind)
            {
                case DateTimeKind.Unspecified:
                    writer.WriteStringValue(DateTime.SpecifyKind(value, DateTimeKind.Local));
                    break;
                case DateTimeKind.Utc:
                    writer.WriteStringValue(value.ToLocalTime());
                    break;
                case DateTimeKind.Local:
                    writer.WriteStringValue(value);
                    break;
                default:
                    throw new NotSupportedException($"DateTimeKind.{value.Kind} not supported");
            }
        }
    }
}
