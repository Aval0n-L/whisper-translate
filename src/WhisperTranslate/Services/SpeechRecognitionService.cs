using OpenAI_API;
using WhisperTranslate.Services.Interfaces;

namespace WhisperTranslate.Services;

public class SpeechRecognitionService : ISpeechRecognitionService
{
    private readonly OpenAIAPI _openAi = new();
    public async Task<string> RecognizeSpeechByFileWithWhisperAsync(string filePath)
    {
        return "test";

        //return await GetTextAsync(filePath);
    }

    public async Task<string> RecognizeSpeechByByteWithWhisperAsync(byte[] audioData)
    {
        var filePath = await SaveAudioToFileAsync(audioData);
        File.Delete(filePath);

        return "test";

        //return await GetTextAsync(filePath);
    }

    private async Task<string> GetTextAsync(string filePath)
    {
        try
        {
            var result = await _openAi.Transcriptions.GetWithDetailsAsync(filePath, "en");

            if (result.segments != null && result.segments.Count > 0)
            {
                Console.WriteLine($"No speech probability: {result.segments[0].no_speech_prob}");
            }
            else
            {
                Console.WriteLine("No segments found.");
            }

            Console.WriteLine(result.ProcessingTime.TotalMilliseconds);
            Console.WriteLine(result.text);
            Console.WriteLine(result.language);
            return result.text;
        }
        catch (Exception exception)
        {
            throw new Exception(exception.Message);
        }
        finally
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    private async Task<string> SaveAudioToFileAsync(byte[] audioData)
    {
        var rootPath = Directory.GetCurrentDirectory();
        var folderPath = Path.Combine(rootPath, "tmp");
        var fileName = $"{Guid.NewGuid()}.m4a";

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var filePath = Path.Combine(folderPath, fileName);

        await File.WriteAllBytesAsync(filePath, audioData);

        return filePath;
    }
}