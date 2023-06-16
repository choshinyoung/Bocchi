using Discord;
using Discord.Interactions;

namespace Bocchi.Interactions;

public class RegistrationModal : IModal
{
    [InputLabel("API key")]
    [ModalTextInput("key", TextInputStyle.Paragraph,
        "이곳에 'sk-...' 형식의 키를 붙여넣으세요.\n해당 키는 안전하게 보관되며,\n사용자님이 봇치를 부를 때만 사용돼요.", 10, 100)]
    public string? Key { get; set; }

    public string Title => "OpenAI API key 등록하기";
}