namespace WhisperTranslate.Services.Interfaces;

public interface ITranslationService
{
    Task<string> TranslateTextWithGptAsync(string text);
}