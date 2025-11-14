using System.ComponentModel;
using System.Runtime.CompilerServices;
using System; // Importar System para Type

namespace Olhuz.Models
{
    public class MenuItemModel : INotifyPropertyChanged
    {
        // Propriedades básicas do item de menu
        public string Title { get; set; }
        public string Icon { get; set; }
        public Type TargetPage { get; set; } // O tipo da página a ser carregada

        // --- Propriedade de Seleção com Notificação ---
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(); // Notifica o XAML que o valor mudou
                }
            }
        }

        // --- Implementação do INotifyPropertyChanged ---
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}