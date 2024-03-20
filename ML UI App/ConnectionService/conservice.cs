using System;
using System.CodeDom;
using System.Configuration;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ML_UI_App.Config;
using ML_UI_App.LogService;
using Serilog.Events;

namespace ML_UI_App.ConnectionService
{
    /// <summary>
    /// Класс для подключения к серверу
    /// </summary>
    internal class ConService
    {
        private static readonly string _ip = "127.0.0.1";
        private static readonly int _port = 9090;
        public static Socket tcpClient = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static bool ContentFlag = false;

        public static void Connect()
        {
            tcpClient.Connect(_ip, _port);
            Logger.LogByTemplate(LogEventLevel.Information, note:"Успешное подключение к серверу");
        }

        public static string Authorization(string operation, string user, string password)
        {
            try
            {
                tcpClient.Send(Encoding.UTF8.GetBytes(operation));
                ListenServer(tcpClient);
                tcpClient.Send(Encoding.UTF8.GetBytes(user));
                ListenServer(tcpClient);
                tcpClient.Send(Encoding.UTF8.GetBytes(password));
                return ListenServer(tcpClient);
            }
            catch (Exception ex)
            {
                Logger.LogByTemplate(LogEventLevel.Error, ex, "Ошибка отправки данных авторизации");
                return "";
            }
            
        }

        public static void SendConfiguration()
        {
            try
            {
                tcpClient.Send(Encoding.UTF8.GetBytes("config"));
                ListenServer(tcpClient);
                tcpClient.Send(Encoding.UTF8.GetBytes($"{Configurator.clientConfig.N} {Configurator.clientConfig.L}"));
                ListenServer(tcpClient);
            }
            catch (Exception ex)
            {
                Logger.LogByTemplate(LogEventLevel.Error, ex, "Ошибка отправки конфигурации.");
            }
            
        }

        public static async Task GetHistory(ListBox listbox)
        {
            ContentFlag = true;
            string tempString;
            try
            {
                tcpClient.Send(Encoding.UTF8.GetBytes("getcontent"));
                ListenServer(tcpClient);
                while (true)
                {
                    if (ContentFlag)
                    {
                        tcpClient.Send(Encoding.UTF8.GetBytes("start"));
                        tempString = ListenServer(tcpClient);
                        if (tempString.Split()[0] == "Компания")
                        {
                            listbox.Items.Clear();
                        }
                        listbox.Items.Add(tempString);
                    }
                    else
                    {
                        tcpClient.Send(Encoding.UTF8.GetBytes("stop"));
                        ListenServer(tcpClient);
                        break;
                    }
                    await Task.Delay(Configurator.clientConfig.Delay);
                }
            }
            catch (Exception ex)
            {
                Logger.LogByTemplate(LogEventLevel.Warning, ex, "Ошибка передачи лора");
            }
            
        }

        public static void StopHistory()
        {
            ContentFlag = false;
        }
        public static string ListenServer(Socket listener)
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

        public static void Disconnect()
        {
            try
            {
                tcpClient.Send(Encoding.UTF8.GetBytes("close"));
                tcpClient.Shutdown(SocketShutdown.Both);
                tcpClient.Close();
                Logger.LogByTemplate(LogEventLevel.Information, note:"Успешное отключение от сервера.");
            }
            catch (Exception ex)
            {
                Logger.LogByTemplate(LogEventLevel.Warning, ex, "Попытка отключиться от сервера без наличия соединения");
            }
            finally
            {
                tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
        }
    }
}
