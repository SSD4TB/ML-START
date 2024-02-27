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
    public partial class Reg : Window
    {
        public Reg()
        {
            InitializeComponent();
        }

        private async void Button_Reg(object sender, RoutedEventArgs e)
        {
            string connectionString = "Server=localhost;Database=authWPF;Trusted_Connection=True;TrustServerCertificate=True";
            //string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=authWPF;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            string insertString = $"INSERT INTO userAuth VALUES (@userlogin, @password);";
            string selectString = $"SELECT count(*) FROM userAuth WHERE userLogin=@userlogin;";

            SqlParameter userlog = new SqlParameter("@userlogin", UserLogin);
            SqlParameter password = new SqlParameter("@password", SecurityAuth.hashPassword(Password));

            if (Password != "" && UserLogin != "")
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    await sqlConnection.OpenAsync();

                    SqlCommand command = new SqlCommand(selectString, sqlConnection);
                    command.Parameters.Add(userlog);
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        if (reader.GetInt32(0) == 0)
                        {
                            await reader.CloseAsync();
                            SqlCommand insertCommand = new SqlCommand(insertString, sqlConnection);
                            insertCommand.Parameters.Add(userlog);
                            insertCommand.Parameters.Add(password);
                            insertCommand.ExecuteNonQuery();
                            MessageBox.Show("Данные успешнно внесены в базу.", "RegAccept", MessageBoxButton.OK, MessageBoxImage.Information);
                            DialogResult = true;
                        }
                        else
                        {
                            MessageBox.Show(@"Данные не записаны.
                        Пользователь уже существует", "RegError", MessageBoxButton.OK ,MessageBoxImage.Error);
                        }
                        await reader.CloseAsync();
                    }
                    else
                    {
                        SqlCommand insertCommand = new SqlCommand(insertString, sqlConnection);
                        insertCommand.Parameters.Add(userlog);
                        insertCommand.Parameters.Add(password);
                        insertCommand.ExecuteNonQuery();
                        MessageBox.Show("Данные успешнно внесены в базу.", "RegAccept", MessageBoxButton.OK, MessageBoxImage.Information);
                        DialogResult = true;
                    }

                    await sqlConnection.CloseAsync();
                }
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
