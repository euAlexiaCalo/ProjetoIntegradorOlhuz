using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Olhuz.Models;
using Olhuz.Views;

namespace Olhuz.Models
{
    // Gerenciar a lista de itens e o estado de seleção
    public class MenuModel : INotifyPropertyChanged
    {
        // ... (Implementação de INotifyPropertyChanged) ...
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Lista de itens do menu
        public ObservableCollection<MenuItemModel> Items { get; set; }

        private MenuItemModel _currentSelectedItem;
        public MenuItemModel CurrentSelectedItem
        {
            get => _currentSelectedItem;
            set => SetProperty(ref _currentSelectedItem, value);
        }

        public MenuModel()
        {
            // Inicializa a lista de itens
            Items = new ObservableCollection<MenuItemModel>
            {
                new MenuItemModel { Title="Menu Principal", Icon="home.png", TargetPage = typeof(HomePage)},
                new MenuItemModel { Title="Ler Imagens", Icon="read_image.png", TargetPage = typeof(ReadImagePage)},
                new MenuItemModel { Title="Leituras Recentes", Icon="reading_history.png", TargetPage = typeof(ReadingHistoryPage)},
                new MenuItemModel { Title="Configurações", Icon="settings.png", TargetPage = typeof(SettingsPage)},
                new MenuItemModel { Title="Minha Conta", Icon="account.png", TargetPage = typeof(MyAccountPage)},
            };
        }
    }
}