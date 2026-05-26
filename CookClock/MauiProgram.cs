using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

using MudBlazor;
using MudBlazor.Services;

namespace CookClock
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
                });

            builder
           .UseMauiApp<App>()
           .ConfigureLifecycleEvents(events =>
           {
#if ANDROID
               events.AddAndroid(android =>
               {
                   android.OnCreate((activity, bundle) =>
                   {
                       var window = activity.Window;

                       if (window is null)
                           return;

                       window.SetStatusBarColor(
                           Android.Graphics.Color.ParseColor("#FFB703"));

                       window.SetNavigationBarColor(
                           Android.Graphics.Color.ParseColor("#FFB703"));
                   });
               });
#endif
           });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddMudServices();
            builder.Services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass =
                    Defaults.Classes.Position.BottomCenter;
            });
            return builder.Build();
        }
    }
}
