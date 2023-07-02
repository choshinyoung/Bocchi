using Bocchi.Functions.Attributes;
using Bocchi.Utility;
using OpenAI;
using OpenAI.Managers;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;

namespace Bocchi.Functions.Functions;

public class ImageFunctions : FunctionModuleBase<FunctionContext>
{
    [Function("GenerateImage",
        "Generate an image with a given prompt and return whether it succeeds.")]
    public async Task<string> GenerateImage(
        [Param("The parameter 'prompt' is a set of appropriate English words for DALL-E image creation.")]
        string prompt)
    {
        using var openAi = new OpenAIService(
            new OpenAiOptions
            {
                ApiKey = Context.ApiKey ?? Config.Get("OPENAI_API_KEY")
            }
        );

        var imageResult = await openAi.Image.CreateImage(new ImageCreateRequest
        {
            Prompt = prompt,
            N = 1,
            Size = StaticValues.ImageStatics.Size.Size256,
            ResponseFormat = StaticValues.ImageStatics.ResponseFormat.Url
        });

        if (imageResult.Successful)
        {
            await Context.Channel.SendMessageAsync(imageResult.Results.First().Url);

            return "{ isSuccess: true }";
        }

        return "{ isSuccess: false }";
    }

    [Function("EditImage",
        "Edit an image with a given prompt and an image and return whether it succeeds.")]
    public async Task<string> EditImage(
        [Param("The parameter 'imageUrl' is a url of the original image that will be edited by AI.")]
        string imageUrl,
        [Param("The parameter 'prompt' is a set of appropriate English words for DALL-E image creation.")]
        string prompt)
    {
        using var openAi = new OpenAIService(
            new OpenAiOptions
            {
                ApiKey = Context.ApiKey ?? Config.Get("OPENAI_API_KEY")
            }
        );

        byte[] originalImage;

        try
        {
            using var client = new HttpClient();
            using var response = await client.GetAsync(imageUrl);

            response.EnsureSuccessStatusCode();

            originalImage = await response.Content.ReadAsByteArrayAsync();
        }
        catch
        {
            return "{ isSuccess: false }";
        }

        var imageResult = await openAi.Image.CreateImageEdit(new ImageEditCreateRequest
        {
            Prompt = prompt,
            N = 1,
            Image = originalImage,
            ImageName = "image",
            Size = StaticValues.ImageStatics.Size.Size256,
            ResponseFormat = StaticValues.ImageStatics.ResponseFormat.Url
        });

        if (!imageResult.Successful)
        {
            return "{ isSuccess: false }";
        }

        await Context.Channel.SendMessageAsync(imageResult.Results.First().Url);

        return "{ isSuccess: true }";
    }
}