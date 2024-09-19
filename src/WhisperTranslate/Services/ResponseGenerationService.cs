using OpenAI_API;
using System.Text.RegularExpressions;
using WhisperTranslate.Services.Interfaces;

namespace WhisperTranslate.Services;

public class ResponseGenerationService : IResponseGenerationService
{
    private readonly OpenAIAPI _openAi = new();

    public async Task<List<string>> GenerateResponsesWithGptASyn(string recognizedText)
    {
        /*var chat = _openAi.Chat.CreateConversation();
        chat.Model = Model.GPT4_Turbo;
        chat.RequestParameters.Temperature = 0;

        chat.AppendSystemMessage($"You are a helpful assistant C#,.Net. Based on the recognized speech: '{recognizedText}', provide 3 possible response options.");
        chat.AppendSystemMessage($"Please, use this pattern: Option 1: answer; Option 2: answer; Option 3: answer;");*/

        var responseFromChatbot = $"Option 1: answer; Option 2: answer; Option 3: answer;";// await chat.GetResponseFromChatbotAsync();

        var formattedResponse = FormatChatbotResponse(responseFromChatbot);

        return formattedResponse;
    }

    private List<string> FormatChatbotResponse(string response)
    {
        string formattedResponse = Regex.Replace(response, "(Option \\d)", "\n$1");

        formattedResponse = formattedResponse.TrimStart();

        var result = formattedResponse.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(option => option.Trim())
            .ToList();

        return result;
    }

}