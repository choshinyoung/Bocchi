using System.Text.Json.Serialization;

namespace Bocchi.Extensions.OpenAi.Models.Request;

public class Parameters
{
    [JsonPropertyName("type")] public string ValueType { get; set; }

    [JsonPropertyName("properties")] public Dictionary<string, Property>? Properties { get; set; }
}