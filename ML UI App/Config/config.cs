using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ML_UI_App.ConnectionService;
using ML_UI_App.LogService;
using static Serilog.Events.LogEventLevel;

namespace ML_UI_App.Config
{
    public class Configurator
    {
        private static readonly string _configFileName = "config.json";
        public static ClientConfig clientConfig = new(7, 5, 2048, 2000);
        public static async Task ChangeConfig(int n = 7, int l = 5, int logmaxsize = 2048, int delay = 2000)
        {
            FileStream filestream = new(_configFileName, FileMode.Create);
            clientConfig = new(n, l, logmaxsize, delay);
            await JsonSerializer.SerializeAsync(filestream, clientConfig);
            Logger.LogByTemplate(Information, note: "Конфигурация обновлена.");
            filestream.Close();
        }

        public static async Task ReadConfig()
        {
            FileStream? filestream;

            try
            {
                filestream = new FileStream(_configFileName, FileMode.Open);
            }
            catch (Exception ex)
            {
                await ChangeConfig();
                filestream = new FileStream(_configFileName, FileMode.Open);
                Logger.LogByTemplate(Error, ex, "Файл конфигурации был создан с нулевыми параметрами из-за его отсутствия");
            }
            try
            {
                clientConfig = await JsonSerializer.DeserializeAsync<ClientConfig>(filestream);
                Logger.LogByTemplate(Error, note:$"{clientConfig.L}, {clientConfig.N}");
            }
            catch (Exception ex)
            {
                filestream.Close();
                await ChangeConfig();
                filestream = new FileStream(_configFileName, FileMode.Open);
                clientConfig = await JsonSerializer.DeserializeAsync<ClientConfig>(filestream);
                Logger.LogByTemplate(Error, ex, "Ошибка чтения конфигурации, сброс настроек до базовых");
            }
            Logger.LogByTemplate(Information, note: "Успешное чтение конфигурации");
            filestream.Close();
        }
    }

    public class ClientConfig(int N, int L, int logmaxsize, int delay)
    {
        public int N { get; } = N;
        public int L { get; set; } = L;
        public int LogMaxSize { get; set; } = logmaxsize;
        public int Delay { get; set; } = delay;
    }
}
