using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using MauiIcons.Fluent;
using Microsoft.Extensions.Logging;

namespace PDF_OCR_Explorer {
    public static class MauiProgram {
        public static MauiApp CreateMauiApp() {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>().UseFluentMauiIcons()
                .UseMauiCommunityToolkit()
                //.UseMauiApp<App>().UseFluentMauiIcons();
                .ConfigureFonts(fonts => {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<IFilePicker>(FilePicker.Default);

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}