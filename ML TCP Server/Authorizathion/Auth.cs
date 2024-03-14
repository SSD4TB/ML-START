using Generic.LogService;
using Microsoft.Data.SqlClient;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using static Serilog.Events.LogEventLevel;

namespace TCPServer.Authorizathion
{
    internal class Auth
    {
        #region Fields
        //TODO: Перенести nameDB в конфигурацию
        public static string nameDB = "ML START";
        private static readonly string conStringForDBCreate = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        private static readonly string connectionString = $@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=""{nameDB}"";Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        #endregion

        #region Public Methods
        public static void CheckDB()
        {
            try
            {
                using SqlConnection sqlConnection = new(conStringForDBCreate);
                sqlConnection.Open();
                SqlCommand sqlCommand = new($"CREATE DATABASE [{nameDB}]", sqlConnection);
                sqlCommand.ExecuteNonQuery();
                Logger.LogByTemplate(Warning, note: "База данных не обнаружена. Создание нового экземпляра БД");
                sqlConnection.Close();
            }
            catch
            {
                Logger.LogByTemplate(Information, note:"База данных существует");
            }
            CheckDBTable();
        }

        //TODO: Улучшить код в регионе авторизации/регистрации.
        public static async Task Login(Socket sender, string username, string password)
        {
            //string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=authWPF;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            string queryString = $"SELECT password FROM userAuth WHERE userLogin=@userlogin;";
            string? value;

            SqlConnection sqlConnection = new(connectionString);
            sqlConnection.Open();

            SqlCommand command = new(queryString, sqlConnection);
            SqlParameter userParam = new("@userlogin", username);
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
                        Logger.LogByTemplate(Information, note: $"{sender.RemoteEndPoint}: успешная авторизация в системе");
                    }
                    else
                    {
                        sender.Send(Encoding.UTF8.GetBytes("FALSE PASSWORD"));
                        Logger.LogByTemplate(Information, note: $"{sender.RemoteEndPoint}: ошибка авторизации в связи с несовпадением данных");
                    }
                }
            }
            else
            {
                sender.Send(Encoding.UTF8.GetBytes("FALSE USER"));
                Logger.LogByTemplate(Information, note: $"{sender.RemoteEndPoint}: ошибка авторизации в связи с отсутствием пользователя в базе данных");
            }
            await sqlConnection.CloseAsync();
        }
        
        public static async Task Registration(Socket sender, string username, string userpass)
        {
            //string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=authWPF;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            string insertString = $"INSERT INTO userAuth VALUES (@userlogin, @password);";
            string selectString = $"SELECT count(*) FROM userAuth WHERE userLogin=@userlogin;";

            SqlParameter userlogin = new("@userlogin", username);
            SqlParameter userlog = new("@userlogin", username);
            SqlParameter password = new("@password", ToHex(userpass));

            SqlConnection sqlConnection = new(connectionString);
            await sqlConnection.OpenAsync();

            SqlCommand command = new(selectString, sqlConnection);
            command.Parameters.Add(userlogin);

            SqlDataReader reader = await command.ExecuteReaderAsync();

            if (reader.HasRows)
            {
                await reader.ReadAsync();
                if (reader.GetInt32(0) == 0)
                {
                    await reader.CloseAsync();
                    SqlCommand insertCommand = new(insertString, sqlConnection);
                    insertCommand.Parameters.Add(userlog);
                    insertCommand.Parameters.Add(password);
                    insertCommand.ExecuteNonQuery();
                    sender.Send(Encoding.UTF8.GetBytes("Данные внесены"));
                    Logger.LogByTemplate(Information, note: $"{sender.RemoteEndPoint}: пользователь зарегистрировал аккаунт");
                }
                else
                {
                    sender.Send(Encoding.UTF8.GetBytes($"Пользователь с логином {username} существует"));
                    Logger.LogByTemplate(Information, note: $"{sender.RemoteEndPoint}: попытка регистрации пользователя, уже существующего в базе данных");
                }
                await reader.CloseAsync();
            }
            else
            {
                SqlCommand insertCommand = new(insertString, sqlConnection);
                insertCommand.Parameters.Add(userlogin);
                insertCommand.Parameters.Add(password);
                insertCommand.ExecuteNonQuery();
                sender.Send(Encoding.UTF8.GetBytes("Данные внесены"));
                Logger.LogByTemplate(Information, note: $"{sender.RemoteEndPoint}: пользователь зарегистрировал аккаунт");
            }

            await sqlConnection.CloseAsync();

        }
        #endregion

        #region Private Methods
        //TODO: Решить, что делать с методом CheckDBTable
        private static void CheckDBTable()
        {
            try
            {
                using SqlConnection sqlConnection = new(connectionString);
                sqlConnection.Open();
                SqlCommand sqlCommand = new("CREATE TABLE userAuth (userLogin VARCHAR(255) PRIMARY KEY NOT NULL, password VARCHAR(255) NOT NULL)", sqlConnection);
                sqlCommand.ExecuteNonQuery();
                Logger.LogByTemplate(Information, note: "Таблица авторизации отсутствовала в базе данных, её создание прошло успешно");
                sqlConnection.Close();
            }
            catch
            {
                Logger.LogByTemplate(Information, note: "Таблица авторизации существует в базе данных");
            }
        }
        private static string ToHex(string defaultString)
        {

            byte[] bytes = Encoding.ASCII.GetBytes(defaultString);
            byte[] hash = MD5.HashData(bytes);

            return $"{Convert.ToHexString(hash)}";
        }
        #endregion
    }
}
