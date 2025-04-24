using Microsoft.Extensions.Logging;
using MSCS533_LocationTracker.Services;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Controls.Hosting;

namespace MSCS533_LocationTracker
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiMaps()
                .ConfigureMauiHandlers(handlers =>
                {
                    // Use fully qualified name to resolve ambiguity
                    handlers.AddHandler<Microsoft.Maui.Controls.Maps.Map, Microsoft.Maui.Maps.Handlers.MapHandler>();
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register services
            builder.Services.AddSingleton<LocationDatabase>();
            builder.Services.AddTransient<MainPage>();

            #if DEBUG
            builder.Logging.AddDebug();
            #endif

            return builder.Build();
        }
    }
}
