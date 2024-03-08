using ML_UI_App.ConnectionService;
using ML_UI_App.Config;
using System.Windows;

namespace ML_UI_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool IsLogin = false;
        private bool IsConnect = false;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Button_DeleteDB(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("скоро эта кнопка совершит величайший прикол =)");
        }

        private void Button_Connect(object sender, RoutedEventArgs e)
        {
            if (IsLogin)
            {
                if (!IsConnect)
                {
                    ConService.Connect();
                    MessageBox.Show("Успешное подключение к серверу", "connection", MessageBoxButton.OK, MessageBoxImage.Information);
                    ConnectToServer();
                }
                else
                {
                    ConService.Disconnect();
                    MessageBox.Show("Отключено", "connection", MessageBoxButton.OK, MessageBoxImage.Information);
                    IsConnect = false;
                    connectButton.Content = "Подключиться";
                }
            }
            else
            {
                ConService.Connect();
                Log logwindow = new();
                if (logwindow.ShowDialog() == true)
                {
                    IsLogin = true;
                    MessageBox.Show("Авторизация прошла успешно.\nСоединение с сервером установлено.", "auth", MessageBoxButton.OK, MessageBoxImage.Information);
                    ConnectToServer();
                }
                else
                {
                    ConService.Disconnect();
                    MessageBox.Show("Ошибка авторизации.", "auth", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Button_Start(object sender, RoutedEventArgs e)
        {
            ConService.Connect();
            IsConnect = true;
        }

        private void Button_Stop(object sender, RoutedEventArgs e)
        {
            ConService.Disconnect();
            IsConnect = false;
        }

        private void testcon_Click(object sender, RoutedEventArgs e)
        {
            if (IsConnect)
            {
                testTextBlock.Text = ConService.TestSendMessage();
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ConService.Disconnect();
        }

        private void ConnectToServer()
        {
            IsConnect = true;
            connectButton.Content = "Отключиться";
            Configurator.ReadFile();
        }
    }
}
