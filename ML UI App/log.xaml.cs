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
                
            }
            else
            {
                if (Password == "" && UserLogin == "")
                {
                    MessageBox.Show("???");
                }
                else
                {
                    if (UserLogin == "")
                    {
                        MessageBox.Show("login");
                    }
                    if (Password == "")
                    {
                        MessageBox.Show("pass?");
                    }
                }
            }
        }

        private void Button_Reg(object sender, RoutedEventArgs e)
        {
            Reg regwindow = new Reg();
            if (regwindow.ShowDialog() == true)
            {
                DialogResult = true;
            }
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
