using Microsoft.AspNetCore.Mvc;
using WhisperTranslate.Services.Interfaces;

namespace WhisperTranslate.Controllers
{
    [ApiController]
    [Route("api/audio")]
    public class AudioController : ControllerBase
    {
        private readonly ISpeechRecognitionService _speechRecognitionService;
        private readonly ITranslationService _translationService;
        private readonly IResponseGenerationService _responseGenerationService;
        private readonly ILogger<AudioController> _logger;

        public AudioController(
            ISpeechRecognitionService speechRecognitionService, 
            ITranslationService translationService, 
            IResponseGenerationService responseGenerationService, 
            ILogger<AudioController> logger)
        {
            _logger = logger;
            _speechRecognitionService = speechRecognitionService;
            _translationService = translationService;
            _responseGenerationService = responseGenerationService;
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> UploadAudio()
        {
            if (!Request.HasFormContentType || Request.Form.Files.Count == 0)
                return BadRequest("File was not uploaded");

            var file = Request.Form.Files[0];
            if (file.Length == 0)
                return BadRequest("File is empty");

            var rootPath = Directory.GetCurrentDirectory();
            var folderPath = Path.Combine(rootPath, ".tmp");
            var fileName = Path.Combine(Guid.NewGuid() + "_" + file.FileName);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var filePath = Path.Combine(folderPath, fileName);
            try
            {
                // Save the audio file to a temporary directory
                await using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Processing an audio file with Whisper API
                var recognizedText = await _speechRecognitionService.RecognizeSpeechByFileWithWhisperAsync(filePath);

                // Text translation with GPT-4
                var translatedText = await _translationService.TranslateTextWithGptAsync(recognizedText);

                // Generating answer options
                var responseOptions = await _responseGenerationService.GenerateResponsesWithGptASyn(recognizedText);

                return Ok(new
                {
                    recognizedText,
                    translatedText,
                    responseOptions
                });
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.Message);
                throw new Exception(exception.Message);
            }
            finally
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
        }
    }
}
