using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration.WindowsSpecific;
using Microsoft.Maui.Devices;
using Application = Microsoft.Maui.Controls.Application;

namespace SessionTrackerProject
{
    public partial class App : Application
    {
        public static double ScaleFactor { get; private set; }

        public App()
        {
            InitializeComponent();

            // Get the main display information
            var mainDisplayInfo = DeviceDisplay.Current.MainDisplayInfo;

            // Now you can access properties like Width, Height, Density, etc.
            double deviceDpi = mainDisplayInfo.Density * 160; // The Density property is the scale factor, not the actual DPI. On Android, the scale factor is relative to 160 dpi.
            double editingDpi = 440;
            ScaleFactor = editingDpi / deviceDpi;

            // Now you can use App.ScaleFactor anywhere in your app to scale your UI elements.
            MainPage = new AppShell();
            UserAppTheme = AppTheme.Dark;
        }
    }
}
