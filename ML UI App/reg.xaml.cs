using System.Windows;
using System.Windows.Controls;

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
            if (Password != "" && UserLogin != "")
            {
                
            }
            else
            {
                MessageBox.Show("Не все данные введены", "RegError", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

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
