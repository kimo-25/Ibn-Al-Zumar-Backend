using System.Text.Json;

namespace IbnAlZumar.API.Ai.Tools
{
    /// <summary>Defensive readers for Gemini function-call arguments (raw JsonElement).</summary>
    public static class AiToolArgExtensions
    {
        public static string? GetStringOrNull(this JsonElement e, string prop)
        {
            if (e.ValueKind != JsonValueKind.Object) return null;
            if (!e.TryGetProperty(prop, out var v)) return null;
            if (v.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined) return null;
            var s = v.ValueKind == JsonValueKind.String ? v.GetString() : v.ToString();
            return string.IsNullOrWhiteSpace(s) ? null : s.Trim();
        }

        public static decimal? GetDecimalOrNull(this JsonElement e, string prop)
        {
            if (e.ValueKind != JsonValueKind.Object || !e.TryGetProperty(prop, out var v)) return null;
            return v.ValueKind switch
            {
                JsonValueKind.Number when v.TryGetDecimal(out var d) => d,
                JsonValueKind.String when decimal.TryParse(v.GetString(), out var d) => d,
                _ => null
            };
        }

        public static int? GetIntOrNull(this JsonElement e, string prop)
        {
            var d = e.GetDecimalOrNull(prop);
            return d.HasValue ? (int)d.Value : null;
        }

        public static bool GetBoolOrDefault(this JsonElement e, string prop, bool defaultValue)
        {
            if (e.ValueKind != JsonValueKind.Object || !e.TryGetProperty(prop, out var v)) return defaultValue;
            return v.ValueKind switch
            {
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.String when bool.TryParse(v.GetString(), out var b) => b,
                _ => defaultValue
            };
        }

        public static List<JsonElement> GetArrayOrEmpty(this JsonElement e, string prop)
        {
            if (e.ValueKind != JsonValueKind.Object) return new List<JsonElement>();
            if (!e.TryGetProperty(prop, out var v) || v.ValueKind != JsonValueKind.Array) return new List<JsonElement>();
            return v.EnumerateArray().ToList();
        }
    }
}