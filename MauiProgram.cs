using Microsoft.Extensions.Logging;
using MSCS533_LocationTracker.Services;
using Microsoft.Maui.Maps;

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
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton(s =>
                new LocationDatabase(Path.Combine(FileSystem.AppDataDirectory, "locations.db3")));

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
