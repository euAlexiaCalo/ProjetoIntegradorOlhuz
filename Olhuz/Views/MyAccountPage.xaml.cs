using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Olhuz.Views
{
    // A classe MyAccountPage deve herdar de ContentPage para ser uma página MAUI
    public partial class MyAccountPage : ContentPage
    {
        public MyAccountPage()
        {
            InitializeComponent();
            // Ao inicializar a página, carregamos os dados do usuário e as informações da app.
            LoadUserData();
        }

        // Método que simula o carregamento dos dados do usuário (Model)
        private void LoadUserData()
        {
            // TODO: Substituir por chamadas a serviços reais de autenticação e dados do usuário.

            // --- SIMULAÇÃO DE DADOS DO USUÁRIO ---
            var user = new
            {
                FullName = "Maria Oliveira da Silva",
                // Usando DateTime para compatibilidade mais ampla, mas DateOnly (disponível em .NET 6+) é preferível.
                BirthDate = new DateTime(1995, 05, 20),
                Email = "maria.os@olhuz.com"
            };

            // --- SEÇÃO 1: INFORMAÇÕES PESSOAIS ---
            // O acesso a NameLabel, BirthDateLabel, etc., é possível porque eles têm x:Name no XAML.
            NameLabel.Text = $"Nome: {user.FullName}";
            BirthDateLabel.Text = $"Data de Nascimento: {user.BirthDate.ToString("dd/MM/yyyy")}";
            EmailLabel.Text = $"Email: {user.Email}";

            // --- SEÇÃO 3: INFORMAÇÕES ADICIONAIS ---
            // Define a versão da aplicação usando o AppInfo do MAUI.
            // Para usar o AppInfo, você precisa adicionar 'using Microsoft.Maui.Devices;'
            string appVersion = AppInfo.Current.VersionString;
            VersaoLabel.Text = $"Versão: {appVersion}";

            // Os textos de Termos e Suporte são definidos primariamente no XAML, 
            // mas podemos garantir que a cor seja sempre a mesma (embora já definida no XAML).
            TermosLabel.TextColor = Colors.Black;
            SuporteLabel.TextColor = Colors.Black;
        }

        // --- SEÇÃO 2: AÇÕES DA CONTA ---

        // Ação para Alterar Senha
        private async void OnChangePasswordClicked(object sender, EventArgs e)
        {
            // Navegação para a página de alteração de senha
            await DisplayAlert("Alterar Senha", "Navegando para a tela de Alteração de Senha...", "OK");
            // Exemplo de navegação real: await Shell.Current.GoToAsync("ChangePasswordPage");
        }

        // Ação para Sair da Conta (Logout)
        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Sair da Conta", "Tem certeza que deseja sair da sua conta?", "Sim", "Não");

            if (confirm)
            {
                // Chamar o serviço de autenticação para realizar o logout (Model)

                await DisplayAlert("Logout", "Você saiu da sua conta com sucesso.", "OK");

                // Redireciona para a página de Login (Usamos "///" para navegação absoluta)
                // Ajuste 'MainPage' para a rota da sua página de Login/Inicial
                await Shell.Current.GoToAsync($"///MainPage");
            }
        }

        // Ação para Excluir Conta - IMPLEMENTAÇÃO NECESSÁRIA
        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            // Etapa 1: Primeira confirmação de segurança.
            bool confirm1 = await DisplayAlert("Excluir Conta",
                "ATENÇÃO: A exclusão da conta é permanente e não pode ser desfeita. Deseja continuar?",
                "Sim, Excluir", "Cancelar");

            if (confirm1)
            {
                // Etapa 2: Confirmação final e mais séria.
                bool confirm2 = await DisplayAlert("Confirmação Final",
                    "Você tem ABSOLUTA certeza que deseja excluir sua conta e todos os seus dados?",
                    "SIM, TENHO CERTEZA", "Cancelar");

                if (confirm2)
                {
                    // TODO: Chamar serviço para excluir a conta do usuário (Model/Service)

                    await Task.Delay(1000); // Simula o tempo de exclusão do backend (1 segundo)

                    await DisplayAlert("Conta Excluída", "Sua conta foi permanentemente excluída. Sentiremos sua falta.", "OK");

                    // Redireciona para a página de Login ou Tela Inicial após a exclusão
                    await Shell.Current.GoToAsync($"///MainPage");
                }
            }
        }

        private async void PoliticaDePrivacidadeTapped(object sender, TappedEventArgs e)
        {
            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("termos.txt");
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

        // Ação: Tocou em 'Suporte'
        private async void SuporteTapped(object sender, TappedEventArgs e)
        {
            await DisplayAlert("Suporte", "Abrindo o chat de suporte ou formulário de contato.", "OK");
        }
    }
}