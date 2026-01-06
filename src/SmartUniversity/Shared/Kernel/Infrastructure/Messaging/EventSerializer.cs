using System.Text;
using System.Text.Json;

namespace SmartUniversity.Shared.Kernel.Infrastructure.Messaging;

public static class EventSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public static byte[] Serialize<T>(T data) =>
        Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data, Options));

    public static T Deserialize<T>(byte[] data) =>
        JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(data), Options)!;
}
