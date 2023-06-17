using System.Text.Json.Serialization;

namespace Bocchi.Extensions.OpenAi.Models.Response;

public class FunctionCall
{
    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("arguments")]
    [JsonExtensionData]
    public Dictionary<string, object> Arguments { get; set; }
}