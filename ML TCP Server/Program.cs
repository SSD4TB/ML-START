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
            tcpSocket.Bind(tcpEndPoint);
            tcpSocket.Listen(5);

            await Host(tcpSocket);
        }

        static async Task Host(Socket tcpSocket)
        {
            Socket listener = tcpSocket.Accept();

            while (true)
            {
            CommunicationBetweenUserAndServer:
                string firstMessage = Listener(listener);
                Console.WriteLine(firstMessage);

                switch (firstMessage)
                {
                    case "auth":
                        {
                            listener.Send(Encoding.UTF8.GetBytes("auth"));
                            string userlogin = Listener(listener);
                            listener.Send(Encoding.UTF8.GetBytes("login"));
                            string password = Listener(listener);
                            Console.WriteLine(userlogin + "\n" + password);
                            await Auth.Login(listener, userlogin, password);
                        }
                        goto CommunicationBetweenUserAndServer;


                    case "reg":
                        {
                            listener.Send(Encoding.UTF8.GetBytes("enter login"));
                            string userlogin = Listener(listener);
                            listener.Send(Encoding.UTF8.GetBytes("enter pass"));
                            string password = Listener(listener);
                            Console.WriteLine(userlogin + "\n" + password);
                            try
                            {
                                await Auth.Reg(listener, userlogin, password);
                            }
                            catch
                            {
                                listener.Send(Encoding.UTF8.GetBytes("error in reg func"));
                            }
                        }
                        goto CommunicationBetweenUserAndServer;
                    case "close":
                        goto Closing;

                    case "help":
                        listener.Send(Encoding.UTF8.GetBytes("reg - registration?\nauth - authorizathion\nhelp - help\nclose - close connect"));
                        goto CommunicationBetweenUserAndServer;

                    case "sendtoclient":
                        await Auth.SendToClientAsync(listener, "message");
                        goto CommunicationBetweenUserAndServer;

                    default:
                        {
                            listener.Send(Encoding.UTF8.GetBytes("idk this command"));
                        }
                        goto CommunicationBetweenUserAndServer;
                }
            }
        Closing:
            listener.Send(Encoding.UTF8.GetBytes("close"));
            listener.Shutdown(SocketShutdown.Both);
            listener.Close();
            //listener.Shutdown(SocketShutdown.Both); // двустороннее закрытие
            //listener.Close();
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