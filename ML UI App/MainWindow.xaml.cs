using ML_UI_App.Config;
using ML_UI_App.ConnectionService;
using ML_UI_App.LogService;
using Serilog.Events;
using System;
using System.Windows;

namespace ML_UI_App;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Logger.CreateLogDirectory(
            LogEventLevel.Debug,
            LogEventLevel.Information,
            LogEventLevel.Warning,
            LogEventLevel.Error
        );
        Closing += MainWindow_Closing;
    }

    private bool IsLogin = false;
    private bool IsConnect = false;

    private void Button_DeleteDB(object sender, RoutedEventArgs e)
    {

    }

    private void Button_Connect(object sender, RoutedEventArgs e)
    {
        if (IsLogin)
        {
            if (!IsConnect)
            {
                try
                {
                    ConService.Connect();
                    MessageBox.Show("Успешное подключение к серверу", "connection", MessageBoxButton.OK, MessageBoxImage.Information);
                    ConnectToServer();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка подключенияя к серверу.", "connection", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                ConService.Disconnect();
                MessageBox.Show("Отключено", "connection", MessageBoxButton.OK, MessageBoxImage.Information);
                IsConnect = false;
                connectButton.Content = "Подключиться";
            }
        }
        else
        {
            try
            {
                ConService.Connect();
                LoginWindow logwindow = new();
                if (logwindow.ShowDialog() == true)
                {
                    IsLogin = true;
                    MessageBox.Show("Авторизация прошла успешно.\nСоединение с сервером установлено.", "auth", MessageBoxButton.OK, MessageBoxImage.Information);
                    ConnectToServer();
                }
                else
                {
                    ConService.Disconnect();
                    MessageBox.Show("Ошибка авторизации.", "auth", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка подключения к серверу.", "connection", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.LogByTemplate(LogEventLevel.Error, ex, "Ошибка подключения к серверу. БИМ БИМ БАМ БАМ");
            }
            
        }
    }

    private async void Button_Start(object sender, RoutedEventArgs e)
    {
        if (IsConnect)
        {
            await ConService.GetHistory(StoryList);
            Logger.LogByTemplate(LogEventLevel.Information, note:"Отправлен запрос на получение лора Незнайки.");
        }
        else
        {
            MessageBox.Show("Подключитесь к серверу перед получением сообщений.", "conservice", MessageBoxButton.OK, MessageBoxImage.Warning);
            Logger.LogByTemplate(LogEventLevel.Warning, note:"Попытка отправки запроса на получение лора Незнайки без подключения к серверу.");
        }
    }

    private void Button_Stop(object sender, RoutedEventArgs e)
    {
        ConService.StopHistory();
    }

    private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        if (IsConnect)
        {
            ConService.Disconnect();
        }
        Logger.LogByTemplate(LogEventLevel.Information, note:"Закрытие программы");
    }

    private void ConnectToServer()
    {
        IsConnect = true;
        connectButton.Content = "Отключиться";
        Configurator.ReadFile();
    }
}
