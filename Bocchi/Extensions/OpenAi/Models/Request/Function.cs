using System.Text.Json.Serialization;

namespace Bocchi.Extensions.OpenAi.Models.Request;

public class Function
{
    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("description")] public string? Description { get; set; }

    [JsonPropertyName("parameters")] public Parameters Parameters { get; set; }

    [JsonPropertyName("required")] public List<string>? Required { get; set; }
}