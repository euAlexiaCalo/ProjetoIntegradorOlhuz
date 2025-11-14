using System;
using System.Linq;
using Microsoft.Maui.Controls;
using Olhuz.Models;

namespace Olhuz.Components
{
    // O Code-Behind atua como Controller
    public partial class SideMenuView : ContentView
    {
        // Instância do nosso Model (dados do menu e lógica de seleção de dados)
        private MenuModel _model;

        public SideMenuView()
        {
            InitializeComponent();

            // 1. Instanciar o Model
            _model = new MenuModel();

            // 2. Ligar o Model à View (BindingContext da View)
            this.BindingContext = _model;

            // 3. Assinar eventos de ciclo de vida e navegação
            this.Loaded += OnSideMenuViewLoaded;
            this.Unloaded += OnSideMenuViewUnloaded;
        }

        private void OnSideMenuViewLoaded(object sender, EventArgs e)
        {
            // O Shell deve estar disponível após o carregamento da View
            if (Shell.Current != null)
            {
                // Garante que o evento seja assinado apenas uma vez
                Shell.Current.Navigated -= Shell_Navigated;
                Shell.Current.Navigated += Shell_Navigated;
            }

            // Dispara a seleção inicial imediatamente
            Shell_Navigated(this, null);
        }

        private void OnSideMenuViewUnloaded(object sender, EventArgs e)
        {
            // Limpa a assinatura para evitar vazamento de memória (memória leak)
            if (Shell.Current != null)
            {
                Shell.Current.Navigated -= Shell_Navigated;
            }
        }

        /// Manipulador do evento de toque no item de menu.
        private async void OnMenuItemTapped(object sender, TappedEventArgs e)
        {
            // A View (Border/TapGestureRecognizer) passa o DataTemplateItem (MenuItemModel)
            if (sender is Border border && border.BindingContext is MenuItemModel item)
            {
                // A navegação e a seleção são tratadas aqui no Controller
                await OnNavigateCommandExecuted(item);
            }
        }

        /// Lógica de seleção do item de menu baseada no item tocado.
        private async Task OnNavigateCommandExecuted(MenuItemModel item)
        {
            if (item == null || item == _model.CurrentSelectedItem) return;

            // 1. Atualiza o estado de seleção no Model
            if (_model.CurrentSelectedItem != null)
            {
                _model.CurrentSelectedItem.IsSelected = false;
            }
            item.IsSelected = true;
            _model.CurrentSelectedItem = item;

            // 2. Executa a navegação (Lógica de Navegação no Controller)
            if (item.TargetPage != null)
            {
                try
                {
                    // Usa o nome da classe como a rota do Shell
                    await Shell.Current.GoToAsync($"//{item.TargetPage.Name}");
                }
                catch (Exception ex)
                {
                    // Lidar com erros de navegação
                    System.Diagnostics.Debug.WriteLine($"Erro de navegação: {ex.Message}");
                }
            }
        }

        /// Lógica de seleção do item de menu baseada na rota atual do Shell (para inicialização e navegação externa).
        private void Shell_Navigated(object sender, ShellNavigatedEventArgs e)
        {
            var currentRoute = Shell.Current?.CurrentState?.Location?.OriginalString;

            if (string.IsNullOrWhiteSpace(currentRoute)) return;

            // Obtém o nome da página (e.g., "HomePage") a partir da rota (e.g., "//HomePage")
            var currentRouteName = currentRoute.Split('/').LastOrDefault();

            if (string.IsNullOrWhiteSpace(currentRouteName)) return;

            // Encontra o item de menu que corresponde à página atual
            var newSelectedItem = _model.Items.FirstOrDefault(item =>
                item.TargetPage != null && item.TargetPage.Name == currentRouteName);

            // Se um novo item for encontrado e for diferente do atual, atualiza o Model
            if (newSelectedItem != null && newSelectedItem != _model.CurrentSelectedItem)
            {
                if (_model.CurrentSelectedItem != null)
                {
                    _model.CurrentSelectedItem.IsSelected = false;
                }

                newSelectedItem.IsSelected = true;
                _model.CurrentSelectedItem = newSelectedItem;
            }
        }
    }
}