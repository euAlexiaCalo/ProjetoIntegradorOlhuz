using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Controls;
using Olhuz.Models;
using Olhuz.Services;

namespace Olhuz.Views
{
    public partial class RegisterPage : ContentPage
    {
        private readonly AuthService _authservice;

        // Estado de controle unificado para as senhas
        private bool isPasswordHidden = true;

        public RegisterPage()
        {
            InitializeComponent();
            // Garante que o botão de cadastro comece desabilitado (agora usando x:Name="RegisterButton")
            BtnCadastrar.IsEnabled = true;

            _authservice = new AuthService();

            // Adiciona listener para habilitar/desabilitar o botão com base no aceite dos termos
            TermsCheckBox.CheckedChanged += OnTermsCheckBoxCheckedChanged;
        }

        // ====================================================================
        // GESTÃO DE SENHA
        // ====================================================================

        // Ação de clique nos ícones do olho (TapGestureRecognizer)
        // ESTE MÉTODO JÁ ALTERNA A VISIBILIDADE E O ÍCONE PARA AMBOS OS CAMPOS, CONFORME SOLICITADO.
        private void OnPasswordToggleTapped(object sender, TappedEventArgs e)
        {
            isPasswordHidden = !isPasswordHidden;

            // Altera o estado de IsPassword para ambos os campos
            PasswordEntry.IsPassword = isPasswordHidden;
            ConfirmPasswordEntry.IsPassword = isPasswordHidden;

            // Altera a imagem para ambos os ícones
            string iconSource = isPasswordHidden ? "eye_closed.png" : "eye_open.png";

            // Note: Usando x:Name="PasswordIcon" e x:Name="ConfirmPasswordIcon"
            PasswordIcon.Source = iconSource;
            ConfirmPasswordIcon.Source = iconSource;
        }

        // Lógica de força da senha (TextChanged no PasswordEntry)
        private void OnPasswordTextChanged(object sender, TextChangedEventArgs e)
        {
            // O sender é o PasswordEntry
            string password = e.NewTextValue ?? string.Empty;

            int strength = 0;
            if (password.Length >= 8) strength++;
            if (password.Any(char.IsUpper)) strength++;
            if (password.Any(char.IsDigit)) strength++;
            if (password.Any(c => !char.IsLetterOrDigit(c))) strength++;

            // Define a cor base conforme o nível de força
            Color barColor = Colors.LightGray;
            string strengthText = "Senha muito fraca";

            switch (strength)
            {
                case 1:
                    barColor = Color.FromArgb("#FF5C5C"); // Vermelho
                    strengthText = "Senha fraca";
                    break;
                case 2:
                    barColor = Color.FromArgb("#FFA500"); // Laranja
                    strengthText = "Senha média";
                    break;
                case 3:
                    barColor = Color.FromArgb("#ADFF2F"); // Verde-amarelado
                    strengthText = "Senha forte";
                    break;
                case 4:
                    barColor = Color.FromArgb("#3CB371"); // Verde
                    strengthText = "Senha muito forte";
                    break;
                default:
                    barColor = Colors.LightGray;
                    strengthText = "Senha muito fraca";
                    break;
            }

            // Atualiza as barras proporcionalmente (usando os x:Name definidos no XAML)
            StrengthBar1.Color = strength >= 1 ? barColor : Colors.LightGray;
            StrengthBar2.Color = strength >= 2 ? barColor : Colors.LightGray;
            StrengthBar3.Color = strength >= 3 ? barColor : Colors.LightGray;
            StrengthBar4.Color = strength >= 4 ? barColor : Colors.LightGray;

            // Atualiza o Label (usando x:Name="PasswordStrengthLabel")
            PasswordStrengthLabel.Text = strengthText;
        }


        // ====================================================================
        // NAVEGAÇÃO E AÇÕES DO FORMULÁRIO
        // ====================================================================

        // Ação do Botão de Voltar (TapGestureRecognizer na seta)
        private async void OnGoBackTapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync(".."); // Volta para a página anterior
        }

        // Manipulador para o CheckBox (Habilita/Desabilita o botão de cadastro)
        private void OnTermsCheckBoxCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            // Habilita o botão se o CheckBox estiver marcado (usando x:Name="RegisterButton")
            BtnCadastrar.IsEnabled = e.Value;
        }

        // Ação do Botão de Cadastro
        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            SetLoading();

            // 1. Verificação de campos vazios/nulos
            if (string.IsNullOrWhiteSpace(FullNameEntry.Text) || string.IsNullOrWhiteSpace(EmailEntry.Text) || string.IsNullOrWhiteSpace(PasswordEntry.Text) || string.IsNullOrWhiteSpace(ConfirmPasswordEntry.Text))
            {
                // Alerta 1: Campos obrigatórios faltando
                await DisplayAlert("Erro de Cadastro", "Por favor, preencha todos os campos obrigatórios.", "OK");
                SetLoading();
                return;
            }

            // 2. Verificação de coincidência de senha
            if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
            {
                // Alerta 2: Senhas não coincidem
                await DisplayAlert("Erro de Senha", "A senha e a confirmação de senha não coincidem.", "OK");
                SetLoading();
                return;
            }

            // 3. Verificação de aceite dos termos
            if (!TermsCheckBox.IsChecked)
            {
                // Alerta 3: Termos não aceitos
                await DisplayAlert("Erro de Cadastro", "Você deve aceitar os Termos de Uso e a Política de Privacidade.", "OK");
                SetLoading();
                return;
            }

            CadastroRequest dadosUsuario = new CadastroRequest
            {

                NomeCompleto = FullNameEntry.Text,
                DataNascimento = BirthDateDatePicker.Date,
                Email = EmailEntry.Text,
                PassWordHash = PasswordEntry.Text

            };

            AuthResponse response = await _authservice.CadastrarUsuarioAsync(dadosUsuario);

            if (response.Erro)
            {
                await DisplayAlert("Erro de Cadastro", response.Message, "OK");
                SetLoading();
                return;
            }

            // Exemplo de sucesso:
            await DisplayAlert("Cadastro Concluído", $"Bem-vindo, {FullNameEntry.Text}! Seu cadastro foi processado. Redirecionando para login.", "OK");

            SetLoading();

            var nome = Uri.EscapeDataString(FullNameEntry.Text);
            await Shell.Current.GoToAsync("//LoginPage");
        }


        // Ação para Termos de Uso (TapGestureRecognizer)
        private async void TermosDeUsoTapped(object sender, TappedEventArgs e)
        {
            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("TermosUso.txt");
                using var reader = new StreamReader(stream);

                var termos = await reader.ReadToEndAsync();
                await DisplayAlert("Termos de Uso", termos, "OK");
            }
            catch (Exception ex)
            {
                // Se o arquivo não for encontrado, exibe uma mensagem de fallback
                await DisplayAlert("Termos de Uso", "Os Termos de Uso não puderam ser carregados. Detalhes: " + ex.Message, "OK");
            }
        }

        // Ação para Política de Privacidade (TapGestureRecognizer)
        private async void PoliticaDePrivacidadeTapped(object sender, TappedEventArgs e)
        {
            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("privacidade.txt");
                using var reader = new StreamReader(stream);

                var politica = await reader.ReadToEndAsync();
                await DisplayAlert("Política de Privacidade", politica, "OK");
            }
            catch (Exception ex)
            {
                // Se o arquivo não for encontrado, exibe uma mensagem de fallback
                await DisplayAlert("Política de Privacidade", "A Política de Privacidade não pôde ser carregada. Detalhes: " + ex.Message, "OK");
            }
        }

        void SetLoading()
        {
            Loading.IsVisible = !Loading.IsVisible;
            BtnCadastrar.IsVisible = !BtnCadastrar.IsVisible;
        }
    }
}