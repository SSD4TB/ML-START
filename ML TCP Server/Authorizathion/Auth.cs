using Microsoft.Data.SqlClient;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace TCPServer.Authorizathion
{
    internal class Auth
    {
        #region ConnectionDBString
        private static readonly string connectionString = @"Data Source=D1232;Initial Catalog=authWPF;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        #endregion

        #region Authorization
        public static async Task Login(Socket sender, string username, string password)
        {
            //string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=authWPF;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            string queryString = $"SELECT count(*) FROM userAuth WHERE userLogin=@userlogin AND password=@password;";
            string? value;

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();

                SqlCommand command = new SqlCommand(queryString, sqlConnection);
                SqlParameter userParam = new SqlParameter("@userlogin", ToHex(username));
                command.Parameters.Add(userParam);
                SqlParameter userPass = new SqlParameter("@password", ToHex(password));
                SqlDataReader reader = await command.ExecuteReaderAsync();

                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        value = reader.GetValue(0).ToString();
                        if (value == ToHex(password))
                        {
                            sender.Send(Encoding.UTF8.GetBytes("true"));
                        }
                        else
                        {
                            sender.Send(Encoding.UTF8.GetBytes("false"));
                        }
                    }
                }
                await sqlConnection.CloseAsync();
            }
        }
        
        public static async Task Registration(Socket sender, string username, string userpass)
        {
            //string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=authWPF;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            string insertString = $"INSERT INTO userAuth VALUES (@userlogin, @password);";
            string selectString = $"SELECT count(*) FROM userAuth WHERE userLogin=@userlogin;";

            SqlParameter userlogin = new SqlParameter("@userlogin", ToHex(username));
            SqlParameter password = new SqlParameter("@password", ToHex(userpass));

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            await sqlConnection.OpenAsync();

            SqlCommand command = new SqlCommand(selectString, sqlConnection);
            command.Parameters.Add(userlogin);
            SqlDataReader reader = await command.ExecuteReaderAsync();

            if (reader.HasRows)
            {
                await reader.ReadAsync();
                if (reader.GetInt32(0) == 0)
                {
                    await reader.CloseAsync();
                    SqlCommand insertCommand = new SqlCommand(insertString, sqlConnection);
                    insertCommand.Parameters.Add(userlogin);
                    insertCommand.Parameters.Add(password);
                    insertCommand.ExecuteNonQuery();
                    sender.Send(Encoding.UTF8.GetBytes("Данные внесены"));
                }
                else
                {
                    sender.Send(Encoding.UTF8.GetBytes("Данные не внесены"));
                }
                await reader.CloseAsync();
            }
            else
            {
                SqlCommand insertCommand = new SqlCommand(insertString, sqlConnection);
                insertCommand.Parameters.Add(userlogin);
                insertCommand.Parameters.Add(password);
                insertCommand.ExecuteNonQuery();
                sender.Send(Encoding.UTF8.GetBytes("Данные внесены"));
            }

            await sqlConnection.CloseAsync();

        }
        #endregion

        #region AuthService
        public static async Task SendToClientAsync(Socket socket, string message)
        {
            await socket.SendAsync(Encoding.UTF8.GetBytes(message));
        }
        public static string ToHex(string defaultString)
        {
            MD5 mD5 = MD5.Create();

            byte[] bytes = Encoding.ASCII.GetBytes(defaultString);
            byte[] hash = mD5.ComputeHash(bytes);

            return $"{Convert.ToHexString(hash)}";
        }
        #endregion
    }
}
