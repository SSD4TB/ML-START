using System.Net;
using System.Net.Sockets;
using System.Text;
using Generic.Config;
using Generic.History;
using TCPServer.Authorizathion;

namespace tcpServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            const string ip = "127.0.0.1";
            const int port = 8080;
            IPEndPoint tcpEndPoint = new(IPAddress.Parse(ip), port);
            Socket tcpSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

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
                        Task.Run(async () => await Host(tcpClient));
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static async Task Host(Socket tcpConnect)
        {
            Console.WriteLine($"Клиент {tcpConnect.RemoteEndPoint} подключился к серверу.");
            Configuration clientConfig = new();

            while (true)
            {
                string firstMessage = Listener(tcpConnect);
                Console.WriteLine($"Клиент {tcpConnect.RemoteEndPoint} отправляет команду {firstMessage}.");

                if (firstMessage == "auth" || firstMessage == "reg")
                { 
                    await Authorization(tcpConnect, firstMessage);
                }
                else if (firstMessage == "getcontent")
                {
                    GetContent(tcpConnect, clientConfig.N, clientConfig.L);
                }
                else if (firstMessage == "close")
                {
                    Console.WriteLine($"Клиент {tcpConnect.RemoteEndPoint} отключился от сервера.");
                    tcpConnect.Shutdown(SocketShutdown.Both);
                }
                else if (firstMessage == "config")
                {
                    tcpConnect.Send(Encoding.UTF8.GetBytes("config"));
                    string[] tempConfig = Listener(tcpConnect).Split();
                    clientConfig.N = int.Parse(tempConfig[0]);
                    clientConfig.L = int.Parse(tempConfig[1]);
                    tcpConnect.Send(Encoding.UTF8.GetBytes("config"));
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

                string firstMessage = Listener(socket);

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

        static async Task Authorization(Socket socket, string typeOperation)
        {
            socket.Send(Encoding.UTF8.GetBytes("1"));
            string userlogin = Listener(socket);
            socket.Send(Encoding.UTF8.GetBytes("1"));
            string password = Listener(socket);

            if (typeOperation == "auth")
            {
                await Auth.Login(socket, userlogin, password);
            }
            else await Auth.Registration(socket, userlogin, password);
        }

        static string Listener(Socket listener)
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
        }
    }
}