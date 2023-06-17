using System.Text.Json.Serialization;

namespace Bocchi.Extensions.OpenAi.Models.Request;

public class Property
{
    [JsonPropertyName("type")] public string Type { get; set; }

    [JsonPropertyName("enum")] public List<string>? Enums { get; set; }

    [JsonPropertyName("description")] public string? Description { get; set; }
}