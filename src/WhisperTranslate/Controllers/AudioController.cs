using System.Text.RegularExpressions;
using WhisperTranslate.Models;
using Microsoft.AspNetCore.Mvc;
using OpenAI_API;
using OpenAI_API.Models;
using OpenAI_API.Moderation;

namespace AudioController.Controllers
{
    [ApiController]
    [Route("api/audio")]
    public class AudioController : ControllerBase
    {
        private readonly OpenAIAPI _openAi;
        private readonly ILogger<AudioController> _logger;

        private static int _counter = 0;

        public AudioController(ILogger<AudioController> logger)
        {
            _openAi = new OpenAIAPI();
            _logger = logger;
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> UploadAudio()
        {
            _counter++;

            if (!Request.HasFormContentType || Request.Form.Files.Count == 0)
                return BadRequest("Файл не был загружен");

            var file = Request.Form.Files[0];
            if (file.Length == 0)
                return BadRequest("Пустой файл");

            var rootPath = Directory.GetCurrentDirectory();
            var tempFolderPath = Path.Combine(rootPath, "tmp");

            var fileName = Path.Combine(Guid.NewGuid() + "-" + file.FileName);

            // Сохранение аудиофайла во временную директорию
            var filePath = Path.Combine(tempFolderPath, fileName);
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Обработка аудиофайла с Whisper API
            var recognizedText = await RecognizeSpeechWithWhisperAsync(filePath);

            // Перевод текста с GPT-4
            var translatedText = await TranslateTextWithGptAsync(recognizedText);

            // Генерация вариантов ответа
            var responseOptions = await GenerateResponsesWithGptASyn(recognizedText);

            // Удаление временного файла
            System.IO.File.Delete(filePath);

            return Ok(new
            {
                RecognizedText = recognizedText,
                TranslatedText = translatedText,
                ResponseOptions = responseOptions
            });
        }

        private async Task<string> RecognizeSpeechWithWhisperAsync(string filePath)
        {
            return $"test + {_counter}";
            
            //var result = await _openAi.Transcriptions.GetWithDetailsAsync(filePath, "en");

            //if (result.segments != null && result.segments.Count > 0)
            //{
            //    // Выводим вероятность отсутствия речи для первого сегмента
            //    Console.WriteLine($"No speech probability: {result.segments[0].no_speech_prob}");
            //}
            //else
            //{
            //    Console.WriteLine("No segments found.");
            //}

            //Console.WriteLine(result.ProcessingTime.TotalMilliseconds);
            //Console.WriteLine(result.text); // ""
            //Console.WriteLine(result.language); // "english"
            //return result.text;
        }

        private async Task<string> TranslateTextWithGptAsync(string text)
        {
            return $"test + {_counter}";
            //var chat = _openAi.Chat.CreateConversation();
            //chat.Model = Model.GPT4_Turbo;
            //chat.RequestParameters.Temperature = 0;

            //chat.AppendSystemMessage($"You are a translation assistant. Translate this text from English to Russian: {text}");

            //var result = await chat.GetResponseFromChatbotAsync();

            //Console.WriteLine(result); // ""

            //return result;
        }

        private async Task<List<string>> GenerateResponsesWithGptASyn(string recognizedText)
        {
            /*var chat = _openAi.Chat.CreateConversation();
            chat.Model = Model.GPT4_Turbo;
            chat.RequestParameters.Temperature = 0;

            chat.AppendSystemMessage($"You are a helpful assistant C#,.Net. Based on the recognized speech: '{recognizedText}', provide 3 possible response options.");
            chat.AppendSystemMessage($"Please, use this pattern: Option 1: answer; Option 2: answer; Option 3: answer;");*/

            var responseFromChatbot = $"Option 1: answer; Option 2: answer; Option 3: answer; Try{_counter}";// await chat.GetResponseFromChatbotAsync();

            var formattedResponse = FormatChatbotResponse(responseFromChatbot);

            return formattedResponse;
        }

        private List<string> FormatChatbotResponse(string response)
        {
            // Шаг 1: Добавляем новую строку перед каждой опцией
            string formattedResponse = Regex.Replace(response, "(Option \\d)", "\n$1");

            // Шаг 2: Убираем начальные пробелы или переносы строки
            formattedResponse = formattedResponse.TrimStart();

            // Шаг 3: Разбиваем строку по новой строке и удаляем пустые строки
            var result = formattedResponse.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(option => option.Trim())
                .ToList();

            return result;
        }
    }
}
