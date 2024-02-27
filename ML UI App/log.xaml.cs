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

        private async void Button_Log(object sender, RoutedEventArgs e)
        {
            string connectionString = "Server=localhost;Database=authWPF;Trusted_Connection=True;TrustServerCertificate=True";
            //string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=authWPF;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            string queryString = $"SELECT password FROM userAuth WHERE userLogin=@userlogin;";
            string value;

            if (UserLogin != "" && Password != "")
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    await sqlConnection.OpenAsync();

                    SqlCommand command = new SqlCommand(queryString, sqlConnection);
                    SqlParameter userParam = new SqlParameter("@userlogin", UserLogin);
                    command.Parameters.Add(userParam);
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            value = Convert.ToString(reader.GetValue(0));
                            if (value == SecurityAuth.hashPassword(Password))
                            {
                                DialogResult = true;
                            }
                            else
                            {
                                MessageBox.Show("Ошибка авторизации");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Ошибка авторизации");
                    }

                    await sqlConnection.CloseAsync();
                }
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
