using System.Windows;
using System.Windows.Controls;
using ML_UI_App.ConnectionService;

namespace ML_UI_App
{
    /// <summary>
    /// Логика взаимодействия для reg.xaml
    /// </summary>
    public partial class Reg : Window
    {
        public Reg()
        {
            InitializeComponent();
        }

        private void Button_Reg(object sender, RoutedEventArgs e)
        {
            if (Password != "" && UserLogin != "" && Password == PasswordRepeat)
            {
                string answer = ConService.Authorization("reg", UserLogin, Password);
                if (answer == "Данные внесены")
                {
                    DialogResult = true;
                }
                else
                {
                    MessageBox.Show($"Ошибка авторизации.\nБолее подробная информация будет в следующей версии\n{answer}", "Регистрация", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                if (Password != PasswordRepeat)
                {
                    MessageBox.Show("Пароли не совпадают", "Reg", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (UserLogin == "" || Password == "")
                {
                    MessageBox.Show("Не все данные были введены", "Reg", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public string Password
        {
            get { return passBox.Password; }
        }

        public string PasswordRepeat
        {
            get { return passBoxTwo.Password; }
        }

        public string UserLogin
        {
            get { return textBoxLogin.Text; }
        }
    }
}
