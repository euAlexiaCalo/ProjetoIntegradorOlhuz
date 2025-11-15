// --------------------------------------------------------------
// Autenticação e navegação em aplicativo MAUI
// --------------------------------------------------------------
using Olhuz.Views;

namespace Olhuz
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
            Routing.RegisterRoute(nameof(ReadImagePage), typeof(ReadImagePage));
            Routing.RegisterRoute(nameof(ReadingHistoryPage), typeof(ReadingHistoryPage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
            Routing.RegisterRoute(nameof(MyAccountPage), typeof(MyAccountPage));
        }
    }
}
