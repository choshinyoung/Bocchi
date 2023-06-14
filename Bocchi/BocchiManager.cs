using OpenAI_API.Models;

namespace Bocchi;

public static class BocchiManager
{
    private const string AssistantMessage =
        "1.인공지능 비서 2.질문 적절+정확 답변 3.출력 메시지 특정 말투 변환+추가 정보x 4.말투 특징\n";

    private const string ConvertSayingMessage =
        "1)말투변환기 2)유저입력>특정말투변환>출력 3)말투특징\n";

    private const string BocchiSaying =
        "1. 소심하고 자신감이 없는 말투이다\n" +
        "2. '아...', '그...', '저...', '그..그건' 처럼 말을 더듬는다\n" +
        "3. 대부분의 완성형 문장을 '...라고 생각해요...', '...인 것 같아요...', '...면 좋을 것 같아요' 처럼 확신 없이 끝낸다 (의문문이나 '그렇군요'같이 간단한 문장은 제외)";

    public static async Task<string> Talk(string content)
    {
        return await Request(AssistantMessage + BocchiSaying, content);
    }

    public static async Task<string> Convert(string content)
    {
        return await Request(ConvertSayingMessage + BocchiSaying, content);
    }

    private static async Task<string> Request(string systemMessage, string content)
    {
        var chat = OpenAi.Api.Chat.CreateConversation();
        chat.Model = Model.GPT4;

        chat.AppendSystemMessage(systemMessage);

        chat.AppendUserInput(content);
        var response = await chat.GetResponseFromChatbotAsync();

        return response;
    }
}