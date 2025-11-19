using Microsoft.Maui.Controls;

namespace Olhuz.Views
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            // Reseta o estado visual dos cards ao voltar à Home
            VisualStateManager.GoToState(ReadImageCard, "Normal");
            VisualStateManager.GoToState(ReadingHistoryCard, "Normal");
            VisualStateManager.GoToState(SettingsCard, "Normal");
            VisualStateManager.GoToState(MyAccountCard, "Normal");
        }

        private async void OnReadImageTapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync($"///ReadImagePage");
        }

        private async void OnReadingHistoryTapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync($"///ReadingHistoryPage");
        }

        private async void OnSettingsTapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync($"///SettingsPage");
        }

        private async void OnMyAccountTapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync($"///MyAccountPage");
        }
    }
}
