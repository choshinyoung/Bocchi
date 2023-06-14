using Bocchi.Utility;
using OpenAI_API;

namespace Bocchi;

public static class OpenAi
{
    public static readonly OpenAIAPI Api = new(new APIAuthentication(Config.Get("OPENAI_API_KEY")));
}