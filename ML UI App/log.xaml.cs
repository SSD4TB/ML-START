using ML_UI_App.ConnectionService;
using System.Windows;

namespace ML_UI_App
{
    /// <summary>
    /// Логика взаимодействия для reg.xaml
    /// </summary>
    public partial class Log : Window
    {
        public Log()
        {
            InitializeComponent();
        }

        private void Button_Log(object sender, RoutedEventArgs e)
        {
            if (UserLogin != "" && Password != "")
            {
                string answer = ConService.Authorization("auth", UserLogin, Password);
                if (answer == "true")
                {
                    DialogResult = true;
                }
                else
                {
                    MessageBox.Show($"Ошибка авторизации.\nБолее подробная информация будет в следующей версии\n{answer}", "Авторизация", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Не все требуемые данные введены", "Авторизация", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Button_Reg(object sender, RoutedEventArgs e)
        {
            var regwindow = new Reg();
            regwindow.ShowDialog();
        }
        
        public string Password
        {
            get { return passBox.Password; }
        }

        public string UserLogin
        {
            get { return textBoxLogin.Text; }
        }
    }
}
