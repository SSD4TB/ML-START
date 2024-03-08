using System.Configuration;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ML_UI_App.Config;

namespace ML_UI_App.ConnectionService
{
    /// <summary>
    /// Класс для подключения к серверу
    /// </summary>
    internal class ConService
    {
        private static readonly string _ip = "127.0.0.1";
        private static readonly int _port = 8080;
        public static Socket tcpClient = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static bool ContentFlag = false;

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
            tcpClient.Send(Encoding.UTF8.GetBytes("config"));
            Listener(tcpClient);
            tcpClient.Send(Encoding.UTF8.GetBytes($"{n} {l}"));
            Listener(tcpClient);
        }

        public static async Task GetHistory(ListBox listbox)
        {
            ContentFlag = true;
            string tempString;
            tcpClient.Send(Encoding.UTF8.GetBytes("getcontent"));
            Listener(tcpClient);
            while (true)
            {
                if (ContentFlag)
                {
                    tcpClient.Send(Encoding.UTF8.GetBytes("start"));
                    tempString = Listener(tcpClient);
                    if(tempString.Split()[0] == "Компания")
                    {
                        listbox.Items.Clear();
                    }
                    listbox.Items.Add(tempString);
                }
                else
                {
                    tcpClient.Send(Encoding.UTF8.GetBytes("stop"));
                    Listener(tcpClient);
                    break;
                }
                await Task.Delay(Configurator.ReadDelay());
            }
        }

        public static void StopHistory()
        {
            ContentFlag = false;
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
