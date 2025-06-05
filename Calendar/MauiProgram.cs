using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Handlers;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using System.Globalization;


#if ANDROID
using Android.Graphics;
#endif
#if WINDOWS
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
#endif

namespace Calendar
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            //CultureInfo.CurrentUICulture = new CultureInfo("en");
            //CultureInfo.CurrentCulture = new CultureInfo("en-US");
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("LexendGiga.ttf", "LexendGiga");
                    fonts.AddFont("Poppins.ttf", "Poppins");
                    fonts.AddFont("Icons.ttf", "Icons");
                });
            EntryHandler.Mapper.AppendToMapping("NoUnderlineOrBorder", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.Background = null;
                handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#endif

#if WINDOWS
    if (handler.PlatformView is TextBox textBox)
    {
        textBox.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
        textBox.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
        textBox.FocusVisualPrimaryThickness = new Microsoft.UI.Xaml.Thickness(0);
        textBox.FocusVisualSecondaryThickness = new Microsoft.UI.Xaml.Thickness(0);
    }
#endif
            });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
