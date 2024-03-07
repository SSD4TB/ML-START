﻿using System;
using System.Collections.Generic;
using System.Linq;
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
            tcpClient.Connect(_ip, _port);
        }

        public static string TestSendMessage()
        {
            tcpClient.Send(Encoding.UTF8.GetBytes("message"));
            return Listener(tcpClient);
        }

        public static void GetHistory()
        {

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
            tcpClient.Close();
        }
    }
}
