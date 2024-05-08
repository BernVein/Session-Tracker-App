using SessionTrackerProject.Views;

namespace SessionTrackerProject
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(FrontPage), typeof(FrontPage));
            Routing.RegisterRoute(nameof(EditSessionPage), typeof(EditSessionPage));
            Routing.RegisterRoute(nameof(AddSessionPage), typeof(AddSessionPage));
            Routing.RegisterRoute(nameof(PDFReaderPage), typeof(PDFReaderPage));

        }
    }
}
