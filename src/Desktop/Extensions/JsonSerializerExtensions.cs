using System.Text.Json;

namespace Noteapp.Desktop.Extensions
{
    public static class JsonSerializerExtensions
    {
        private readonly static JsonSerializerOptions _options = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IgnoreReadOnlyProperties = true
        };

        public static string ToJson<T>(this T value)
        {
            return JsonSerializer.Serialize(value, _options);
        }

        public static T FromJson<T>(this string json)
        {
            return JsonSerializer.Deserialize<T>(json, _options);
        }
    }
}
