using System.Xml;

namespace Olhuz.Views
{
    public partial class MyAccountPage : ContentPage
    {
        public MyAccountPage()
        {
            InitializeComponent();
            // Ao inicializar a página, carregamos os dados do usuário.
            // Isso simula a função do Controller de buscar dados do Model.
            LoadUserData();
        }

        // Método que simula o carregamento dos dados do usuário (Model)
        private void LoadUserData()
        {
            // Chamar serviço para obter os dados reais do usuário logado.

            // Dados Fictícios para demonstração:
            var user = new
            {
                FullName = "Maria Oliveira",
                BirthDate = new DateOnly(1995, 05, 20),
                Email = "maria.oliveira@exemplo.com"
            };

            // Atualiza a View (Labels) com os dados carregados:
            WelcomeLabel.Text = $"Olá, {user.FullName}";
            NameLabel.Text = $"Nome: {user.FullName}";
            BirthDateLabel.Text = $"Data de Nascimento: {user.BirthDate.ToString("dd/MM/yyyy")}";
            EmailLabel.Text = $"Email: {user.Email}";
        }

        // Ação para Alterar Senha
        private async void OnChangePasswordClicked(object sender, EventArgs e)
        {
            // Navegação para a página de alteração de senha
            await DisplayAlert("Alterar Senha", "Navegando para a tela de Alteração de Senha...", "OK");
            // Exemplo de navegação: await Shell.Current.GoToAsync("ChangePasswordPage");
        }

        // Ação para Sair da Conta (Logout)
        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Sair da Conta", "Tem certeza que deseja sair da sua conta?", "Sim", "Não");

            if (confirm)
            {
                // Chamar o serviço de autenticação para realizar o logout (Model)

                await DisplayAlert("Logout", "Você saiu da sua conta com sucesso.", "OK");

                // Redireciona para a página de Login
                await Shell.Current.GoToAsync($"///MainPage");
            }
        }
    }
}