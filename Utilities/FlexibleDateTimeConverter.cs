using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

namespace CardZen.Utilities;

public class FlexibleDateTimeConverter : JsonConverter<DateTime>
{
    private readonly string[] _formats = { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd" };

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return default;

        string? dateString = reader.GetString();

        // Xử lý chuỗi rỗng hoặc chỉ có khoảng trắng
        if (string.IsNullOrWhiteSpace(dateString)) return default;

        // Thử parse với các định dạng đã định nghĩa
        if (DateTime.TryParseExact(dateString, _formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
        {
            return result;
        }

        // Nếu không khớp định dạng nào, thử parse mặc định trước khi bỏ cuộc
        if (DateTime.TryParse(dateString, out DateTime defaultResult))
        {
            return defaultResult;
        }

        return default;
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy-MM-dd HH:mm:ss"));
    }
}

// Converter cho nullable
public class FlexibleNullableDateTimeConverter : JsonConverter<DateTime?>
{
    private readonly FlexibleDateTimeConverter _inner = new();

    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return null;
        return _inner.Read(ref reader, typeof(DateTime), options);
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            _inner.Write(writer, value.Value, options);
        else
            writer.WriteNullValue();
    }
}