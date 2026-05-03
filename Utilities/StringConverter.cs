using System.Text.Json;
using System.Text.Json.Serialization;

namespace CardZen.Utilities
{
    public class FlexibleStringConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Nếu là chuỗi, trả về chuỗi đó
            if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString() ?? string.Empty;
            }

            // Nếu là số, chuyển đổi số đó thành chuỗi
            if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetInt64(out long l)) return l.ToString();
                if (reader.TryGetDouble(out double d)) return d.ToString();
            }

            // Nếu là Boolean (true/false)
            if (reader.TokenType == JsonTokenType.True || reader.TokenType == JsonTokenType.False)
            {
                return reader.GetBoolean().ToString().ToLower();
            }

            // Mặc định trả về chuỗi rỗng thay vì gây lỗi parse
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                return doc.RootElement.GetRawText().Trim('"');
            }
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}