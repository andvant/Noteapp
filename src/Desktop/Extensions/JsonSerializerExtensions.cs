using System.Text.Json;

namespace Noteapp.Desktop.Extensions
{
    public static class JsonSerializerExtensions
    {
        private static JsonSerializerOptions options = new() { WriteIndented = true };

        public static string ToJson<T>(this T value)
        {
            return JsonSerializer.Serialize(value, options);
        }

        public static T FromJson<T>(this string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
