using Olhuz.Services;
using Olhuz.Models;
using System.Runtime.CompilerServices;

namespace Olhuz.Views
{
    public partial class LoginPage : ContentPage
    {
        //Criar variavel que recebe a instancia;
        private readonly AuthService _authservice;

        public LoginPage()
        {
            InitializeComponent();
            //Inicializo a varável com a instância do serviço que criamos
            _authservice = new AuthService();
        }

        // 1. Ação para o botão de voltar
        private async void OnGoBackTapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync(".."); // Volta para a página anterior
        }

        // 2. Ação para o botão de Login
        private async void OnLoginClicked(object sender, EventArgs e)
        {
            SetLoading();

            // Simples: apenas valida campos preenchidos
            if (string.IsNullOrWhiteSpace(EmailEntry.Text) || string.IsNullOrWhiteSpace(PasswordEntry.Text))
            {
                await DisplayAlert("Erro", "Por favor, preencha todos os campos.", "OK");
                SetLoading();
                return;
            }

            LoginRequest dadosLogin = new LoginRequest
            {
                Email = EmailEntry.Text,
                PassWordHash = PasswordEntry.Text
            };

            AuthResponse response = await _authservice.LoginAsync(dadosLogin);

            if (response.Erro)
            {
                await DisplayAlert("Erro", response.Message, "OK");
                SetLoading();
                return;
            }

            // Sucesso: Redireciona para a home page
            await DisplayAlert("Sucesso", "Tentativa de Login bem-sucedida!", "OK");
            SetLoading();
            await Shell.Current.GoToAsync($"{nameof(HomePage)}");
        }

        void SetLoading()
        {
            Loading.IsVisible = !Loading.IsVisible;
            BtnEntrar.IsVisible = !BtnEntrar.IsVisible;
        }

        // 3. Ação para alternar a visibilidade da senha
        private void OnTogglePasswordVisibilityTapped(object sender, TappedEventArgs e)
        {
            // Inverte o estado atual da propriedade IsPassword
            PasswordEntry.IsPassword = !PasswordEntry.IsPassword;

            // Altera a imagem do ícone
            if (PasswordEntry.IsPassword)
            {
                PasswordIcon.Source = "eye_closed.png";
            }
            else
            {
                PasswordIcon.Source = "eye_open.png";
            }
        }

        // 4. Ação para redefinir senha (Link)
        private async void OnForgotPasswordTapped(object sender, TappedEventArgs e)
        {
            // Lógica para navegação para a página de Redefinir Senha
            await DisplayAlert("Redefinir Senha", "Navegando para a página de recuperação de senha...", "OK");
            // await Shell.Current.GoToAsync("ForgotPasswordPage");
        }
    }
}