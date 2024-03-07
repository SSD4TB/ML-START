using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ML_UI_App.ConnectionService
{
    /// <summary>
    /// Класс для подключения к серверу
    /// </summary>
    internal class ConService
    {
        // TODO: Реализовать подключение к серверу через конфигурацию/UI приложение
        private static readonly string _ip = "127.0.0.1";
        private static readonly int _port = 8080;
        public static Socket tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public static void Connect()
        {
            Task.Run(async() => await tcpClient.ConnectAsync(_ip, _port));
        }

        public static string TestSendMessage()
        {
            tcpClient.Send(Encoding.UTF8.GetBytes("message"));
            return Listener(tcpClient);
        }

        public static string Authorization(string operation, string user, string password)
        {
            tcpClient.Send(Encoding.UTF8.GetBytes(operation));
            Listener(tcpClient);
            tcpClient.Send(Encoding.UTF8.GetBytes(user));
            Listener(tcpClient);
            tcpClient.Send(Encoding.UTF8.GetBytes(password));
            return Listener(tcpClient);
        }

        public static void SendConfiguration(int n, int l)
        {
            tcpClient.Send(Encoding.UTF8.GetBytes($"{n} {l}"));
        }

        public static void GetHistory()
        {
            // 1
        }
        public static string Listener(Socket listener)
        {
            var buffer = new byte[256];
            var size = 0;
            var data = new StringBuilder();

            do
            {
                size = listener.Receive(buffer);
                data.Append(Encoding.UTF8.GetString(buffer, 0, size));
            } while (listener.Available > 0);

            return data.ToString();
        }

        public static void Disconnect()
        {
            tcpClient.Send(Encoding.UTF8.GetBytes("close"));
            tcpClient.Shutdown(SocketShutdown.Both);
            tcpClient.Close();
            tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
    }
}
