namespace Olhuz.Views
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
        }
        
        private async void OnReadImageTapped(object sender, TappedEventArgs e)
        {
            // Lógica para navegar para a página de Ler Imagens
            await Shell.Current.GoToAsync($"{nameof(ReadImagePage)}");
        }

        private async void OnReadingHistoryTapped(object sender, TappedEventArgs e)
        {
            // Lógica para navegar para a página de Leituras Recentes
            await Shell.Current.GoToAsync($"{nameof(ReadingHistoryPage)}");
        }

        private async void OnSettingsTapped(object sender, TappedEventArgs e)
        {
            // Lógica para navegar para a página de Configurações
            await Shell.Current.GoToAsync($"{nameof(SettingsPage)}");
        }

        private async void OnMyAccountTapped(object sender, TappedEventArgs e)
        {
            // Lógica para navegar para a página de Minha Conta
            await Shell.Current.GoToAsync($"{nameof(MyAccountPage)}");
        }
    }
}