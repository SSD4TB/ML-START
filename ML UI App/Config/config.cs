using System;
using System.IO;
using System.IO.Pipes;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ML_UI_App.ConnectionService

namespace ML_UI_App.Config
{
    internal class Configurator
    {
        static async Task CreateFile()
        {
            FileStream filestream = new("config.json", FileMode.OpenOrCreate);
            Configuration conf = new(7, 5, 2048, 500);
            await JsonSerializer.SerializeAsync<Configuration>(filestream, conf);
            Console.WriteLine("Конфигурация восстановлена.");
            filestream.Close();
        }

        public static async Task ReadFile(Socket socket)
        {
            FileStream? filestream;

            try
            {
                filestream = new FileStream("config.json", FileMode.Open);
            }
            catch (Exception ex)
            {
                await CreateFile();
                filestream = new FileStream("config.json", FileMode.Open);
            }
            Configuration? config;
            try
            {
                config = await JsonSerializer.DeserializeAsync<Configuration>(filestream);
            }
            catch (Exception ex)
            {
                filestream.Close();
                await CreateFile();
                filestream = new FileStream("config.json", FileMode.Open);
                config = await JsonSerializer.DeserializeAsync<Configuration>(filestream);

            }
            ConService.SendConfiguration(config.N, config.L);
            filestream.Close();
        }
    }

    class Configuration
    {
        public Configuration(int N, int L, int logmaxsize, int delay)
        {
            this.N = N;
            this.L = L;
            LogMaxSize = logmaxsize;
            Delay = delay;
        }
        public int N { get; }
        public int L { get; set; }
        public int LogMaxSize { get; set; }
        public int Delay { get; set; }
    }
}
