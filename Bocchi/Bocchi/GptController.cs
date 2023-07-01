using Bocchi.Functions;
using Bocchi.Utility;
using OpenAI;
using OpenAI.Managers;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels.ResponseModels;
using OpenAI.ObjectModels.SharedModels;

namespace Bocchi;

public class GptController
{
    public static readonly List<string> Prefixes = Setting.GetList<string>("PREFIXES");

    public static async Task<string> TalkAsync(string content, FunctionContext context, List<History>? histories = null,
        string? apiKey = null)
    {
        return await RequestAsync(StaticMessages.AssistantMessage + StaticMessages.Saying, content, context, histories,
            apiKey);
    }

    public static async Task<string> ConvertAsync(string content, FunctionContext context, string? apiKey = null)
    {
        return await RequestAsync(StaticMessages.ConvertSayingMessage + StaticMessages.Saying, content, context,
            apiKey: apiKey);
    }

    private static async Task<string> RequestAsync(string systemMessage, string content, FunctionContext context,
        List<History>? histories = null, string? apiKey = null)
    {
        var messages = new List<ChatMessage>
        {
            ChatMessage.FromSystem(systemMessage)
        };

        if (histories != null)
        {
            messages.AddRange(histories.Select(h =>
                new ChatMessage(
                    h.IsAssistant ? StaticValues.ChatMessageRoles.Assistant : StaticValues.ChatMessageRoles.User,
                    h.Content)));
        }

        messages.Add(ChatMessage.FromUser(content));

        return await RequestMessagesAsync(messages, apiKey, context);
    }

    private static async Task<string> RequestMessagesAsync(List<ChatMessage> messages, string? apiKey,
        FunctionContext context)
    {
        using var openAi = new OpenAIService(
            new OpenAiOptions
            {
                ApiKey = apiKey ?? Config.Get("OPENAI_API_KEY")
            }
        );

        for (var stackCount = 0; stackCount < 10; stackCount++)
        {
            var result = await GetResponse(messages, openAi);

            if (result.FinishReason != "function_call")
            {
                return result.Message.Content;
            }

            await ProcessFunctionCall(messages, context, result);
        }

        throw new Exception("Too many function recall occured.");
    }

    private static async Task<ChatChoiceResponse> GetResponse(List<ChatMessage> messages, OpenAIService openAi)
    {
        var functions = BocchiManager.FunctionManager.Functions.Select(f => f.Function).ToList();

        var model = (await openAi.ListModel()).Models
            .Any(m => m.Id == Models.Gpt_4_0613)
                ? Models.Gpt_4_0613
                : Models.Gpt_3_5_Turbo_0613;

        var response = await openAi.CreateCompletion(new ChatCompletionCreateRequest
        {
            Messages = messages,
            Model = model,
            Functions = functions
        });

        return CheckResponse(response);
    }

    private static ChatChoiceResponse CheckResponse(ChatCompletionCreateResponse response)
    {
        if (response.Successful)
        {
            return response.Choices.First();
        }

        if (response.Error == null)
        {
            throw new Exception("Unknown Error");
        }

        throw new Exception($"{response.Error.Code}: {response.Error.Message}");
    }

    private static async Task ProcessFunctionCall(List<ChatMessage> messages, FunctionContext context,
        ChatChoiceResponse result)
    {
        result.Message.Content ??= string.Empty;
        messages.Add(result.Message);

        var function = FindFunction(result);

        messages.Add(ChatMessage.FromFunction(
            await BocchiManager.FunctionManager.ExecuteFunction(result.Message.FunctionCall!, context),
            function.Name
        ));
    }

    private static FunctionDefinition FindFunction(ChatChoiceResponse choice)
    {
        var function = BocchiManager.FunctionManager.SearchFunction(choice.Message.FunctionCall!.Name!);

        if (function == null)
        {
            throw new Exception($"Function {choice.Message.FunctionCall!.Name} not exists.");
        }

        return function;
    }
}