using System.Globalization;

namespace WhisperTranslate;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors(); 

        services.AddHealthChecks();

        services
            .AddControllers()
            .AddJsonOptions(o => { o.JsonSerializerOptions.PropertyNamingPolicy = null; });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        //configure CORS if you have public endpoints;
        app.UseCors(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().SetPreflightMaxAge(TimeSpan.FromDays(1)));

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
        });
    }
}