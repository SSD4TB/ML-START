using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1
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
