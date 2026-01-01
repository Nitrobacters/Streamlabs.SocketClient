using System.Text.Json;
using System.Text.Json.Serialization;
using Streamlabs.SocketClient.Events.Abstractions;
using Streamlabs.SocketClient.Converters;

namespace Streamlabs.SocketClient.InternalExtensions;

internal static class SerializationExtensions
{
    private static readonly JsonSerializerOptions Options = new()
    {
        AllowTrailingCommas = false,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip,
    };

    static SerializationExtensions()
    {
        Options.Converters.Add(new FlexibleStringConverter());
        Options.Converters.Add(new IntStringConverter());
    }

    private static readonly IReadOnlyCollection<IStreamlabsEvent> Empty = Array.Empty<IStreamlabsEvent>();

    public static IReadOnlyCollection<IStreamlabsEvent> Deserialize(this string json)
    {
        string normalized = json.NormalizeTypeDiscriminators();
        return JsonSerializer.Deserialize<IReadOnlyCollection<IStreamlabsEvent>>(normalized, Options) ?? Empty;
    }
}
