using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sql;
using System.Security.Cryptography;
using Microsoft.Data.SqlClient;
using System.Net;
using System.Net.Sockets;

namespace TCPClientServer.Authorizathion
{
    internal class Auth
    {
        private static readonly string connectionString = @"Data Source=D1232;Initial Catalog=authWPF;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        public static async Task Login(Socket sender, string username, string password)
        {
            //string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=authWPF;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            string queryString = $"SELECT password FROM userAuth WHERE userLogin=@userlogin;";
            string value;

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();

                SqlCommand command = new SqlCommand(queryString, sqlConnection);
                SqlParameter userParam = new SqlParameter("@userlogin", username);
                command.Parameters.Add(userParam);
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

        public static async Task SendToClientAsync(Socket socket, string message)
        {
            await socket.SendAsync(Encoding.UTF8.GetBytes(message));
        }
        
        public static async Task Reg(Socket sender, string username, string userpass)
        {
            //string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=authWPF;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            string insertString = $"INSERT INTO userAuth VALUES (@userlogin, @password);";
            string selectString = $"SELECT count(*) FROM userAuth WHERE userLogin=@userlogin;";

            SqlParameter userlog = new SqlParameter("@userlogin", username);
            SqlParameter userlogin = new SqlParameter("@userlogin", username);
            SqlParameter password = new SqlParameter("@password", ToHex(userpass));

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
                        insertCommand.Parameters.Add(userlog);
                        insertCommand.Parameters.Add(password);
                        insertCommand.ExecuteNonQuery();
                        sender.Send(Encoding.UTF8.GetBytes("Данные внесены"));
                    }

                    await sqlConnection.CloseAsync();
                }
            
        }

        public static string ToHex(string password)
        {
            MD5 mD5 = MD5.Create();

            byte[] bytes = Encoding.ASCII.GetBytes(password);
            byte[] hash = mD5.ComputeHash(bytes);

            return $"{Convert.ToHexString(hash)}";
        }
    }
}
