namespace WhisperTranslate.Services.Interfaces;

public interface ISpeechRecognitionService
{
    Task<string> RecognizeSpeechByFileWithWhisperAsync(string filePath);

    Task<string> RecognizeSpeechByByteWithWhisperAsync(byte[] audioData);
}