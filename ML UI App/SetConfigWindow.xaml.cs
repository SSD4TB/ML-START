using ML_UI_App.Config;
using ML_UI_App.LogService;
using System;
using System.Windows;
using static Serilog.Events.LogEventLevel;

namespace ML_UI_App
{
    /// <summary>
    /// Логика взаимодействия для SetConfigWindow.xaml
    /// </summary>
    public partial class SetConfigWindow : Window
    {
        public SetConfigWindow()
        {
            InitializeComponent();
            textBoxNumN.Text = $"{Configurator.clientConfig.N}";
            textBoxNumL.Text = $"{Configurator.clientConfig.L}";
            textBoxDelay.Text = $"{Configurator.clientConfig.Delay}";
        }

        private async void Button_SetConfigure(object sender, RoutedEventArgs e)
        {
            try
            {
                await Configurator.ChangeConfig(NumN, NumL, delay: Delay);
                MessageBox.Show("Изменения сохранены.\nДля применения чисел N и L переподключитесь к серверу.", "config", MessageBoxButton.OK, MessageBoxImage.Information);
                Logger.LogByTemplate(Information, note:$"Обновление конфигурации: установлены значения N={NumN}, L={NumL}, Задержка={Delay}");
            }
            catch (Exception ex)
            {
                Logger.LogByTemplate(Warning, ex, "");
                MessageBox.Show("Проверьте правильность введённых данных.", "configuration", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public int NumN
        {
            get { return int.Parse(textBoxNumN.Text); }
        }

        public int NumL
        {
            get { return int.Parse(textBoxNumL.Text); }
        }

        public int Delay
        {
            get { return int.Parse(textBoxDelay.Text); }
        }
    }
}
