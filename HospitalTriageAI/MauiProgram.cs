using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using HospitalTriageAI.Data;
using HospitalTriageAI.Data.Repositories;
using HospitalTriageAI.Services;
using HospitalTriageAI.AI;

namespace HospitalTriageAI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // Add Blazor WebView
        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        // === DATABASE ===
        var dbPath = AppDbContext.GetDatabasePath();
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        // === REPOSITORIES ===
        builder.Services.AddScoped<IPatientRepository, PatientRepository>();

        // === SERVICES ===
        builder.Services.AddScoped<IPatientService, PatientService>();
        builder.Services.AddScoped<ITriageService, TriageService>();

        // === AI ===
        builder.Services.AddSingleton<TriagePredictionEngine>();

        var app = builder.Build();

        // Ensure database is created
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        }

        return app;
    }
}
