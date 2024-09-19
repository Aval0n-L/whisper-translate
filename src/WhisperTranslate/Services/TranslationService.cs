using OpenAI_API;
using WhisperTranslate.Services.Interfaces;

namespace WhisperTranslate.Services;

public class TranslationService : ITranslationService
{
    private readonly OpenAIAPI _openAi = new();
    public async Task<string> TranslateTextWithGptAsync(string text)
    {
        return $"test";
        //var chat = _openAi.Chat.CreateConversation();
        //chat.Model = Model.GPT4_Turbo;
        //chat.RequestParameters.Temperature = 0;

        //chat.AppendSystemMessage($"You are a translation assistant. Translate this text from English to Russian: {text}");

        //var result = await chat.GetResponseFromChatbotAsync();

        //Console.WriteLine(result); // ""

        //return result;
    }

}