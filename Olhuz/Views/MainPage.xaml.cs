namespace Olhuz.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            // Lógica para navegação para a página de Login
            await Shell.Current.GoToAsync($"{nameof(LoginPage)}");
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            // Lógica para navegação para a página de Cadastro
            await Shell.Current.GoToAsync($"{nameof(RegisterPage)}");
        }
    }
}