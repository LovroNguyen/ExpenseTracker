using ExpenseTracker.Services;
using ExpenseTracker.Views;
using ExpenseTracker.ViewModels;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker
{
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
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // HTTP Client
            builder.Services.AddSingleton<HttpClient>();

            // Configuration
            builder.Services.AddSingleton<FirebaseConfig>();

            // Services
            builder.Services.AddSingleton<FirebaseService>();

            // ViewModels
            builder.Services.AddSingleton<ProjectsViewModel>();
            builder.Services.AddTransient<ProjectDetailsViewModel>();
            builder.Services.AddTransient<AddExpenseViewModel>();

            // Views
            builder.Services.AddSingleton<ProjectsPage>();
            builder.Services.AddTransient<ProjectDetailsPage>();
            builder.Services.AddTransient<AddExpensePage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
