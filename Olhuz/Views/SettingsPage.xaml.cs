namespace Olhuz.Views

{

    public partial class SettingsPage : ContentPage

    {

        // Variáveis de estado do Controller (simulando o Model)

        private bool _isScreenReaderEnabled = true;

        private double _speechSpeed = 1.0;

        private string _voiceType = "Masculino";

        private string _currentTheme = "Claro"; // Claro, Escuro



        // Cores para destacar o botão ativo - Serão carregadas dos recursos

        private Color _activeButtonColor;

        private Color _inactiveButtonColor;

        // NOVAS VARIÁVEIS para cores de texto baseadas na sua regra

        private Color _activeTextColor; // Cor do texto para botão ativo (Fundo SevenBlue)

        private Color _inactiveTextColor; // Cor do texto para botão inativo



        public SettingsPage()

        {

            InitializeComponent();



            // Define o Title da página

            Title = "Configurações";



            // Chamada para carregar as cores dos recursos XAML antes de aplicar as configurações

            LoadButtonColorsFromResources();



            // Carrega e aplica as configurações iniciais do "Model"

            LoadCurrentSettings();

        }



        /// Tenta carregar as cores de destaque dos botões a partir do dicionário de recursos XAML

        private void LoadButtonColorsFromResources()

        {

            // 1. Cor para o fundo ATIVO (SevenBlue) e Cor do texto INATIVO

            // (Mantém a lógica existente)

            _activeButtonColor = GetResourceColor("LightPrimaryButton", Color.FromArgb("#3884CF"));

            _inactiveTextColor = Colors.White;



            // 3. Cor do texto ATIVO (White, já definido como padrão no campo, mas para clareza)

            _activeTextColor = Colors.White;



            // 💡 Determina a chave e a cor padrão com base no tema atual

            var currentTheme = Application.Current.RequestedTheme;

            string inactiveKey;

            Color defaultInactiveColor;



            if (currentTheme == AppTheme.Dark)

            {

                // Tema Escuro: usa a chave

                inactiveKey = "DarkSecondaryButton";

                // Padrão para tema escuro, se a chave não for encontrado

                defaultInactiveColor = Color.FromArgb("#5D6065");

            }

            else // AppTheme.Light ou AppTheme.Unspecified

            {

                // Tema Claro: usa a chave

                inactiveKey = "LightSecondaryButton";

                // Padrão para tema claro

                defaultInactiveColor = Color.FromArgb("#B8B8B8");

            }



            // Carrega a cor do recurso com a chave apropriada

            _inactiveButtonColor = GetResourceColor(inactiveKey, defaultInactiveColor);

        }



        /// Método auxiliar para carregar uma cor pelo seu x:Key no ResourceDictionary.

        private Color GetResourceColor(string key, Color defaultColor)

        {

            if (Application.Current.Resources.TryGetValue(key, out object resourceValue) && resourceValue is Color color)

            {

                return color;

            }

            // Se a chave não for encontrada ou o valor não for uma cor, retorna o padrão.

            return defaultColor;

        }



        // Simula o carregamento das configurações salvas (do Model/Preferências)

        private void LoadCurrentSettings()

        {

            // Na aplicação real, estas variáveis seriam carregadas de um armazenamento persistente (e.g., Preferences, Banco de Dados).



            // Aplica os valores na View

            ScreenReaderSwitch.IsToggled = _isScreenReaderEnabled;

            VolumeSlider.Value = 0.5; // Valor inicial do Slider (0.0 a 1.0)



            // Atualiza o estado visual da View

            UpdateSpeechSpeedButtons(_speechSpeed);

            UpdateVoiceTypeButtons(_voiceType);

            UpdateThemeButtons(_currentTheme);

        }



        // ====================================================================

        // GESTÃO DE EVENTOS DA VIEW

        // ====================================================================



        // 1. Leitura de Tela (SWITCH)

        private void OnScreenReaderToggled(object sender, ToggledEventArgs e)

        {

            _isScreenReaderEnabled = e.Value;



            // Salvar _isScreenReaderEnabled no Model/Preferências



        }



        // 2. Velocidade da Fala (BOTÕES)

        private void OnSpeechSpeedClicked(object sender, EventArgs e)

        {

            if (sender is Button button && button.CommandParameter is string speedString)

            {

                if (double.TryParse(speedString, out double newSpeed))

                {

                    _speechSpeed = newSpeed;



                    // Salvar _speechSpeed no Model/Preferências



                    UpdateSpeechSpeedButtons(newSpeed);

                }

            }

        }



        // 3. Tipo de Voz (BOTÕES)

        private void OnVoiceTypeClicked(object sender, EventArgs e)

        {

            if (sender is Button button && button.CommandParameter is string newVoice)

            {

                _voiceType = newVoice;



                // Salvar _voiceType no Model/Preferências



                UpdateVoiceTypeButtons(newVoice);

            }

        }



        // 4. Volume (SLIDER e +/- BOTÕES)

        private void OnVolumeSliderValueChanged(object sender, ValueChangedEventArgs e)

        {

            // O volume é atualizado continuamente pelo slider

            double newVolume = e.NewValue;



            // Salvar o valor do volume (newVolume) no Model/Preferências

        }



        private void OnVolumeClicked(object sender, EventArgs e)

        {

            if (sender is Button button && button.CommandParameter is string direction)

            {

                double step = 0.1; // Ajuste em 10%

                double currentVolume = VolumeSlider.Value;



                if (direction == "Up")

                {

                    VolumeSlider.Value = Math.Min(1.0, currentVolume + step);

                }

                else if (direction == "Down")

                {

                    VolumeSlider.Value = Math.Max(0.0, currentVolume - step);

                }



                // O evento ValueChanged do Slider tratará o salvamento.

            }

        }



        // 5. Modo de Exibição (BOTÕES)

        private void OnThemeClicked(object sender, EventArgs e)

        {

            if (sender is Button button && button.CommandParameter is string newTheme)
            {
                _currentTheme = newTheme;

                // Salva a preferência do tema

                if (newTheme == "Claro")
                {
                    Application.Current.UserAppTheme = AppTheme.Light;
                    // FALLBACK VISUAL: Garante que o fundo da página mude caso o XAML não esteja a aplicar os recursos de cor Light.
                    this.BackgroundColor = GetResourceColor("LightBackground", Color.FromArgb("#E3F2FD")); // Cor base para tema claro
                }

                else if (newTheme == "Escuro")
                {
                    Application.Current.UserAppTheme = AppTheme.Dark;
                    // FALLBACK VISUAL: Garante que o fundo da página mude caso o XAML não esteja a aplicar os recursos de cor Dark.
                    this.BackgroundColor = GetResourceColor("DarkBackground", Color.FromArgb("#1F222B"));
                }

                // Recarrega as cores dos botões inativos
                LoadButtonColorsFromResources();

                // Atualiza o destaque visual dos botões
                UpdateThemeButtons(newTheme);

                UpdateSpeechSpeedButtons(_speechSpeed);
                UpdateVoiceTypeButtons(_voiceType);
            }
        }



        // ====================================================================
        // ATUALIZAÇÕES VISUAIS AUXILIARES (Controller manipula a View)
        // ====================================================================



        // Destaca o botão de velocidade ativo
        private void UpdateSpeechSpeedButtons(double activeSpeed)
        {
            // Usa a referência direta ao elemento nomeado no XAML (SpeedButtonsLayout)

            var layout = SpeedButtonsLayout;

            if (layout == null) return;

            foreach (var child in layout.Children)

            {

                if (child is Button button && button.Text.EndsWith("x"))

                {

                    if (double.TryParse(button.CommandParameter as string, out double buttonSpeed) && buttonSpeed == activeSpeed)

                    {

                        // ESTADO ATIVO: Fundo e Texto

                        button.BackgroundColor = _activeButtonColor;

                        button.TextColor = _activeTextColor;

                    }

                    else

                    {

                        // ESTADO INATIVO: Fundo e Texto

                        button.BackgroundColor = _inactiveButtonColor;

                        button.TextColor = _inactiveTextColor;

                    }

                }

            }

        }



        // Destaca o botão de tipo de voz ativo

        private void UpdateVoiceTypeButtons(string activeVoice)

        {

            // Usa a referência direta ao elemento nomeado no XAML (VoiceTypeButtonsLayout)

            var layout = VoiceTypeButtonsLayout;



            if (layout == null) return;



            foreach (var child in layout.Children)

            {

                if (child is Button button)

                {

                    if (button.Text == activeVoice)

                    {

                        // ESTADO ATIVO: Fundo SevenBlue, Texto White

                        button.BackgroundColor = _activeButtonColor;

                        button.TextColor = _activeTextColor;

                    }

                    else

                    {
                        // ESTADO INATIVO: Fundo White, Texto SevenBlue

                        button.BackgroundColor = _inactiveButtonColor;
                        button.TextColor = _inactiveTextColor;
                    }
                }
            }
        }

        // Destaca o botão de tema ativo
        private void UpdateThemeButtons(string activeTheme)
        {
            // Usa a referência direta ao elemento nomeado no XAML (ThemeButtonsLayout)
            var layout = ThemeButtonsLayout;

            if (layout == null) return;
            foreach (var child in layout.Children)

            {

                if (child is Button button)

                {

                    // Compara o texto do botão com o tema ativo (ex: "Claro" vs "Claro")

                    if (button.Text == activeTheme)
                    {
                        // ESTADO ATIVO: Fundo e texto
                        button.BackgroundColor = _activeButtonColor;
                        button.TextColor = _activeTextColor;

                    }

                    else
                    {
                        // ESTADO INATIVO: Fundo e texto
                        button.BackgroundColor = _inactiveButtonColor;
                        button.TextColor = _inactiveTextColor;
                    }
                }
            }
        }
    }
}