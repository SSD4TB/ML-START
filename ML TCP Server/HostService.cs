using System.Net;
using System.Net.Sockets;
using System.Text;
using Generic.Config;
using Generic.History;
using Generic.LogService;
using TCPServer.Authorizathion;
using static Serilog.Events.LogEventLevel;

namespace ML_TCP_Server.HostService
{
    class HostServer
    {
        #region Fields
        //TODO: Вынести IP и port в конфигурацию
        private static string ip = "127.0.0.1";
        private static int port = 8080;
        private static IPEndPoint tcpEndPoint = new(IPAddress.Parse(ip), port);
        private static Socket tcpSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        #endregion

        private static void SetConfig()
        {
            try
            {
                ServerConfiguration server = ServerConfigurator.GetConfig().Result;
                ip = server.IP;
                port = server.Port;
                tcpEndPoint = new(IPAddress.Parse(ip), port);
                tcpSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Auth.SetConfig(server.NameDB);
                Logger.LogByTemplate(Information, note:"Конфигурация применена.");
            }
            catch(Exception ex)
            {
                Logger.LogByTemplate(Warning, ex, "Использована стандартная конфигурация");
            }
        }

        #region Handler Methods
        public static async Task RunHost()
        {
            SetConfig();
            Auth.CheckDB();
            WriteToConsole("Настройка параметров завершена, запуск сервера...");

            try
            {
                tcpSocket.Bind(tcpEndPoint);
                tcpSocket.Listen();
                WriteToConsole($"Сервер запущен [{ip}:{port}]. Ожидание подключений... ");

                while (true)
                {
                    var tcpClient = await tcpSocket.AcceptAsync();

                    try
                    {
                        Task.Run(async () => await ProcessClient(tcpClient));
                    }
                    catch (Exception ex)
                    {
                        Logger.LogByTemplate(Error, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteToConsole(ex.Message);
                Logger.LogByTemplate(Error, ex);
            }
        }

        static async Task ProcessClient(Socket tcpConnect)
        {
            WriteToConsole($"Клиент {tcpConnect.RemoteEndPoint} отправил запрос на подключение к серверу.");
            ClientConfiguration clientConfig = new();
            Logger.LogByTemplate(Information, note:$"По адресу {tcpConnect.RemoteEndPoint} был подключен клиент");

            while (tcpConnect.Connected)
            {
                string command = ListenClient(tcpConnect);
                WriteToConsole($"Клиент {tcpConnect.RemoteEndPoint} отправляет команду {command}.");
                Logger.LogByTemplate(Information, note:$"{tcpConnect.RemoteEndPoint}: отправлена команда {command}");
                

                if (command == "auth" || command == "reg")
                {
                    await Authorization(tcpConnect, command);
                }
                else if (command == "getcontent")
                {
                    Logger.LogByTemplate(Information, note: $"{tcpConnect.RemoteEndPoint}: отправлен запрос на получение лора незнайки");
                    GetContent(tcpConnect, clientConfig.N, clientConfig.L);
                }
                else if (command == "close")
                {
                    WriteToConsole($"Клиент {tcpConnect.RemoteEndPoint} отключился от сервера.");
                    Logger.LogByTemplate(Information, note: $"{tcpConnect.RemoteEndPoint}: закрывает соединение");
                    tcpConnect.Shutdown(SocketShutdown.Both);
                }
                else if (command == "config")
                {
                    tcpConnect.Send(Encoding.UTF8.GetBytes("config"));
                    string[] tempConfig = ListenClient(tcpConnect).Split();
                    clientConfig.N = int.Parse(tempConfig[0]);
                    clientConfig.L = int.Parse(tempConfig[1]);
                    tcpConnect.Send(Encoding.UTF8.GetBytes("config"));
                    Logger.LogByTemplate(Information, note: $"{tcpConnect.RemoteEndPoint}: отправлена конфигурация");
                }
                else
                {
                    tcpConnect.Send(Encoding.UTF8.GetBytes("cmd"));
                }
            }
        }

        static void GetContent(Socket socket, int configN, int configL)
        {
            socket.Send(Encoding.UTF8.GetBytes("start get content"));
            string[] story = History.Speak(configN, configL);
            int i = 0;
            while (true)
            {
                if (i == story.Length)
                {
                    i = 0;
                    story = History.Speak(configN, configL);
                }

                string firstMessage = ListenClient(socket);

                if (firstMessage == "start")
                {
                    socket.Send(Encoding.UTF8.GetBytes(story[i]));
                    i++;
                }
                else if (firstMessage == "close")
                {
                    WriteToConsole($"Клиент {socket.RemoteEndPoint} отключился от сервера.");
                    socket.Send(Encoding.UTF8.GetBytes("close"));
                    socket.Shutdown(SocketShutdown.Both);
                    Logger.LogByTemplate(Information, note: $"{socket.RemoteEndPoint}: закрывает соединение при передаче лора");
                    break;
                }
                else if (firstMessage == "stop")
                {
                    socket.Send(Encoding.UTF8.GetBytes("stop get content"));
                    WriteToConsole($"Клиент {socket.RemoteEndPoint} отправляет команду stop.");
                    break;
                }
                else
                {
                    socket.Send(Encoding.UTF8.GetBytes("cmd"));
                }
            }
        }
        #endregion

        #region Handler Helper Methods
        static async Task Authorization(Socket socket, string typeOperation)
        {
            socket.Send(Encoding.UTF8.GetBytes("1"));
            string userlogin = ListenClient(socket);
            socket.Send(Encoding.UTF8.GetBytes("1"));
            string password = ListenClient(socket);

            if (typeOperation == "auth")
            {
                await Auth.Login(socket, userlogin, password);
                Logger.LogByTemplate(Information, note: $"{socket.RemoteEndPoint}: отправлен запрос на авторизацию");
            }
            else
            {
                await Auth.Registration(socket, userlogin, password);
                Logger.LogByTemplate(Information, note: $"{socket.RemoteEndPoint}: отправлен запрос на регистрацию");
            }
        }

        static string ListenClient(Socket listener)
        {
            var buffer = new byte[256];
            var data = new StringBuilder();

            do
            {
                int size = listener.Receive(buffer);
                data.Append(Encoding.UTF8.GetString(buffer, 0, size));
            } while (listener.Available > 0);

            return data.ToString();
        }

        private static void WriteToConsole(string message)
        {
            Console.WriteLine($"[{DateTime.Now}]: {message}");
        }
        #endregion
    }
}