using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using UraniumUI;
using SessionTrackerProject.Views;
using Plugin.LocalNotification;

namespace SessionTrackerProject
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseUraniumUI()
                .UseUraniumUIMaterial()
                //Needed configuration for the notification
                .UseLocalNotification(config =>
                {
                    config.AddCategory(new NotificationCategory(NotificationCategoryType.Status)
                    {
                        ActionList = new HashSet<NotificationAction>(new List<NotificationAction>()
                                    {
                                        new NotificationAction(100)
                                        {
                                                Title = "Open",
                                                Android =
                                                {
                                                    LaunchAppWhenTapped = true,
                                                    IconName =
                                                    {
                                                            ResourceName = "i2"
                                                    }
                                                }
                                        },
                                        new NotificationAction(101)
                                        {
                                                Title = "Dismiss",
                                                Android =
                                                {
                                                    LaunchAppWhenTapped = false,
                                                    IconName =
                                                    {
                                                            ResourceName = "i3"
                                                    }
                                                }
                                        }
                                    })
                    });
                })
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("LexendExa-Bold.ttf", "LexendExa-Bold");
                fonts.AddFont("LexendExa-Regular.ttf", "LexendExa-Regular");
                fonts.AddFont("LexendZetta-Regular.ttf", "LexendZetta-Regular");
                fonts.AddFont("LexendExa-Bold.ttf", "LexendExa-Bold");
                fonts.AddFont("LexendExa-Thin.ttf", "LexendExa-Thin");
                fonts.AddFont("GuifxV2Transports-YMJo.ttf", "MediaFont");
                fonts.AddFont("Product Sans Bold Italic.ttf", "GoogleFont-BoldItalic");
                fonts.AddFont("Product Sans Bold.ttf", "GoogleFont-Bold");
                fonts.AddFont("Product Sans Italic.ttf", "GoogleFont-Italic");
                fonts.AddFont("Product Sans Regular.ttf", "GoogleFont-Regular");
            }).UseMauiCommunityToolkit();
            builder.Services.AddTransient<EditSessionPage>();
            builder.Services.AddTransient<AddSessionPage>();
#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}