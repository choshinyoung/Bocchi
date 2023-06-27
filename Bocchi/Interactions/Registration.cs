using Bocchi.Utility;
using Bocchi.Utility.Database;
using Discord.Interactions;
using OpenAI;
using OpenAI.Managers;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;

namespace Bocchi.Interactions;

public class Registration : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("등록", "OpenAI API KEY 등록하기")]
    public async Task Register()
    {
        await Context.Interaction.RespondWithModalAsync<RegistrationModal>("register");
    }

    [ModalInteraction("register")]
    public async Task RegisterModal(RegistrationModal modal)
    {
        await DeferAsync();

        if (!await ValidateKey(modal.Key))
        {
            await FollowupAsync(StaticMessages.InvalidKeyMessage);

            return;
        }

        var user = DbManager.GetUser(Context.User.Id);
        user.OpenAiKey = modal.Key!;
        user.IsTrial = false;

        DbManager.SetUser(Context.User.Id, u => u.OpenAiKey!, user.OpenAiKey);
        DbManager.SetUser(Context.User.Id, u => u.IsTrial, user.IsTrial);

        await FollowupAsync(StaticMessages.KeyUpdatedMessage);
    }

    private static async Task<bool> ValidateKey(string? key)
    {
        if (key is null)
        {
            return false;
        }

        using var openAi = new OpenAIService(new OpenAiOptions
        {
            ApiKey = key
        });

        try
        {
            var result = await openAi.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
            {
                Messages = new List<ChatMessage>
                {
                    ChatMessage.FromUser("안녕하세요")
                },
                Model = Models.Gpt_3_5_Turbo
            });

            return result.Successful;
        }
        catch
        {
            return false;
        }
    }
}