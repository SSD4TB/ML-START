using System;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TCPClientServer.Authorizathion;

namespace tcpServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            const string ip = "127.0.0.1";
            const int port = 8080;
            IPEndPoint tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            var tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                tcpSocket.Bind(tcpEndPoint);
                tcpSocket.Listen();
                Console.WriteLine("Сервер запущен. Ожидание подключений... ");

                while (true)
                {
                    var tcpClient = await tcpSocket.AcceptAsync();

                    await Host(tcpClient);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static async Task Host(Socket tcpSocket)
        {
            Socket listener = tcpSocket.Accept();

            while (true)
            {
                string firstMessage = Listener(listener);
                Console.WriteLine(firstMessage);

                if (firstMessage == "auth" || firstMessage == "reg")
                { 
                    await SignInUp(listener, firstMessage);
                }
                else if (firstMessage == "getcontent")
                {
                    // ml generic => wpfapp
                }
                else if (firstMessage == "close")
                {
                    listener.Send(Encoding.UTF8.GetBytes("close"));
                    listener.Shutdown(SocketShutdown.Both);
                    listener.Close();
                }
                else
                {
                    listener.Send(Encoding.UTF8.GetBytes("cmd"));
                }
            }
        }

        static async Task SignInUp(Socket socket, string typeOperation)
        {
            socket.Send(Encoding.UTF8.GetBytes("1"));
            string userlogin = Listener(socket);
            socket.Send(Encoding.UTF8.GetBytes("1"));
            string password = Listener(socket);

            if (typeOperation == "auth")
            {
                await Auth.Login(socket, userlogin, password);
            }
            else await Auth.Reg(socket, userlogin, password);
        }

        static string Listener(Socket listener)
        {
            var buffer = new byte[256];
            var size = 0;
            var data = new StringBuilder();

            do
            {
                size = listener.Receive(buffer); // получение данных, в size количество реально полученных байт
                data.Append(Encoding.UTF8.GetString(buffer, 0, size));
            } while (listener.Available > 0);

            return data.ToString();
        }

    }
}