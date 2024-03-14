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
        private static readonly string ip = "127.0.0.1";
        private static readonly int port = 8080;
        private static readonly IPEndPoint tcpEndPoint = new(IPAddress.Parse(ip), port);
        private static readonly Socket tcpSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        #endregion

        #region Handler Methods
        public static async Task RunHost()
        {
            //TODO: Сделать серверную конфигурацию
            Auth.CheckDB();
            try
            {
                tcpSocket.Bind(tcpEndPoint);
                tcpSocket.Listen();
                Console.WriteLine("Сервер запущен. Ожидание подключений... ");

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
                Console.WriteLine(ex.Message);
            }
        }

        static async Task ProcessClient(Socket tcpConnect)
        {
            Console.WriteLine($"Клиент {tcpConnect.RemoteEndPoint} отправил запрос на подключение к серверу.");
            ClientConfiguration clientConfig = new();
            Logger.LogByTemplate(Information, note:$"По адресу {tcpConnect.RemoteEndPoint} был подключен клиент");

            while (true)
            {
                string command = ListenClient(tcpConnect);
                Console.WriteLine($"Клиент {tcpConnect.RemoteEndPoint} отправляет команду {command}.");
                Logger.LogByTemplate(Information, note:$"{tcpConnect.RemoteEndPoint}: отправлена команда {command}");
                

                if (command == "auth" || command == "reg")
                {
                    await Authorization(tcpConnect, command);
                }
                else if (command == "getcontent")
                {
                    GetContent(tcpConnect, clientConfig.N, clientConfig.L);
                    Logger.LogByTemplate(Information, note: $"{tcpConnect.RemoteEndPoint}: отправлен запрос на получение лора незнайки");
                }
                else if (command == "close")
                {
                    Console.WriteLine($"Клиент {tcpConnect.RemoteEndPoint} отключился от сервера.");
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
                    Disconnect(socket);
                    break;
                }
                else if (firstMessage == "stop")
                {
                    socket.Send(Encoding.UTF8.GetBytes("stop get content"));
                    Console.WriteLine($"Клиент {socket.RemoteEndPoint} отправляет команду stop.");
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

        private static void Disconnect(Socket socket)
        {
            socket.Send(Encoding.UTF8.GetBytes("close"));
            socket.Shutdown(SocketShutdown.Both);
            Logger.LogByTemplate(Information, note: $"{socket.RemoteEndPoint}: закрывает соединение при передаче лора");
        }
        #endregion
    }
}