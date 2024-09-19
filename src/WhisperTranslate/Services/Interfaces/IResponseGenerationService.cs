namespace WhisperTranslate.Services.Interfaces;

public interface IResponseGenerationService
{
    Task<List<string>> GenerateResponsesWithGptASyn(string recognizedText);
}