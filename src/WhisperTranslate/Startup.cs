using System.Globalization;
using WhisperTranslate.Services;
using WhisperTranslate.Services.Interfaces;
using WhisperTranslate.WebSockets;

namespace WhisperTranslate;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors();

        services.AddHealthChecks();

        services.AddHttpClient();

        services.AddSingleton<ISpeechRecognitionService, SpeechRecognitionService>();
        services.AddSingleton<ITranslationService, TranslationService>();
        services.AddSingleton<IResponseGenerationService, ResponseGenerationService>();

        services
            .AddControllers()
            .AddJsonOptions(o => { o.JsonSerializerOptions.PropertyNamingPolicy = null; });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        //configure CORS if you have public endpoints;
        app.UseCors(p => p.
            AllowAnyOrigin().
            AllowAnyHeader().
            AllowAnyMethod().
            WithExposedHeaders("Content-Disposition").
            SetPreflightMaxAge(TimeSpan.FromDays(1)
            )
        );

        app.UseWebSockets();
        app.UseRouting();

        // these lines only if you have authentication and authorization set up
        // app.UseAuthentication();
        // app.UseAuthorization();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGet("api/ping", context => context.Response.WriteAsync(
                DateTime.UtcNow.ToString("s", CultureInfo.InvariantCulture), context.RequestAborted));
            
            endpoints.Map("/ws", async context =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    var webSocketHandler = new WebSocketHandler(
                        app.ApplicationServices.GetRequiredService<ISpeechRecognitionService>(),
                        app.ApplicationServices.GetRequiredService<ITranslationService>(),
                        app.ApplicationServices.GetRequiredService<IResponseGenerationService>()
                        );
                    await webSocketHandler.HandleWebSocketAsync(webSocket);
                }
                else
                {
                    context.Response.StatusCode = 400;
                }
            });
        });
    }
}