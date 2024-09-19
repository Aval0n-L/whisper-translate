using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using WhisperTranslate.Services.Interfaces;

namespace WhisperTranslate.WebSockets;

public class WebSocketHandler
{
    private readonly ISpeechRecognitionService _speechRecognitionService;
    private readonly ITranslationService _translationService;
    private readonly IResponseGenerationService _responseGenerationService;

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="speechRecognitionService"></param>
    /// <param name="translationService"></param>
    /// <param name="responseGenerationService"></param>
    public WebSocketHandler(
        ISpeechRecognitionService speechRecognitionService,
        ITranslationService translationService,
        IResponseGenerationService responseGenerationService)
    {
        _speechRecognitionService = speechRecognitionService;
        _translationService = translationService;
        _responseGenerationService = responseGenerationService;
    }

    public async Task HandleWebSocketAsync(WebSocket webSocket)
    {
        var buffer = new byte[4 * 1024];

        try
        {
            while (webSocket.State == WebSocketState.Open)
            {
                var receivedData = new List<byte>();
                WebSocketReceiveResult result;

                // Receive data from the client side
                do
                {
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by the WebSocket client", CancellationToken.None);
                        return;
                    }

                    receivedData.AddRange(buffer.Take(result.Count));
                }
                while (!result.EndOfMessage);

                // Processing binary messages (audio data)
                if (result.MessageType == WebSocketMessageType.Binary)
                {
                    byte[] audioData = receivedData.ToArray();

                    // Speech recognition
                    var recognizedText = await _speechRecognitionService.RecognizeSpeechByByteWithWhisperAsync(audioData);

                    // We use the service to translate text
                    var translatedText = await _translationService.TranslateTextWithGptAsync(recognizedText);

                    // We use the service to generate answer options
                    var responseOptions = await _responseGenerationService.GenerateResponsesWithGptASyn(recognizedText);

                    var response = new
                    {
                        recognizedText,
                        translatedText,
                        responseOptions
                    };
                    var responseJson = JsonSerializer.Serialize(response);
                    var responseBuffer = Encoding.UTF8.GetBytes(responseJson);

                    await webSocket.SendAsync(new ArraySegment<byte>(responseBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
                else
                {
                    // todo
                    Console.WriteLine("Received message of unsupported type.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            if (webSocket.State == WebSocketState.Open)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Server error", CancellationToken.None);
            }
        }
    }
}