using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Streamlabs.SocketClient.Converters;

/// <summary>
/// Converts loose numeric payloads into integers without throwing when values are malformed.
/// </summary>
internal sealed class IntStringConverter : JsonConverter<int>
{
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Number when reader.TryGetInt64(out long numberValue):
                return (int)numberValue;
            case JsonTokenType.Number:
                return (int)Math.Round(reader.GetDouble(), MidpointRounding.AwayFromZero);
            case JsonTokenType.String:
                return ParseString(reader.GetString());
            case JsonTokenType.True:
                return 1;
            case JsonTokenType.False:
            case JsonTokenType.Null:
            case JsonTokenType.Undefined:
                return 0;
            default:
                using (JsonDocument document = JsonDocument.ParseValue(ref reader))
                {
                    return ParseString(document.RootElement.ToString());
                }
        }
    }

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
    }

    private static int ParseString(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return 0;
        }

        if (int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out int parsed))
        {
            return parsed;
        }

        if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double doubleValue))
        {
            return (int)Math.Round(doubleValue, MidpointRounding.AwayFromZero);
        }

        return 0;
    }
}
