using System;
using System.IO;
using System.IO.Pipes;
using System.Text.Json;
using System.Threading.Tasks;

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

        static async Task ReadFile()
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

            Console.WriteLine($"N: {config?.N}  L: {config?.L}  Log: {config?.LogMaxSize}  Delay: {config?.Delay}");
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
