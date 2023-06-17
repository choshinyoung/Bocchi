using System.Text.Json.Serialization;
using OpenAI.ObjectModels.ResponseModels;
using OpenAI.ObjectModels.SharedModels;

namespace Bocchi.Extensions.OpenAi.Models.Response;

public record ChatCompletionWithFunctionCreateResponse : BaseResponse, IOpenAiModels.IId, IOpenAiModels.ICreatedAt
{
    [JsonPropertyName("model")] public string Model { get; set; }

    [JsonPropertyName("choices")] public List<ChatChoiceResponse> Choices { get; set; }

    [JsonPropertyName("usage")] public UsageResponse Usage { get; set; }

    [JsonPropertyName("created")] public int CreatedAt { get; set; }

    [JsonPropertyName("id")] public string Id { get; set; }
}