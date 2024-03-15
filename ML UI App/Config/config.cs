using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ML_UI_App.ConnectionService;

namespace ML_UI_App.Config
{
    public class Configurator
    {
        static async Task CreateFile()
        {
            FileStream filestream = new("config.json", FileMode.OpenOrCreate);
            ClientConfig conf = new(7, 5, 2048, 1000);
            await JsonSerializer.SerializeAsync<ClientConfig>(filestream, conf);
            Console.WriteLine("Конфигурация восстановлена.");
            filestream.Close();
        }

        public static async Task ReadFile()
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
            ClientConfig? config;
            try
            {
                config = await JsonSerializer.DeserializeAsync<ClientConfig>(filestream);
            }
            catch (Exception ex)
            {
                filestream.Close();
                await CreateFile();
                filestream = new FileStream("config.json", FileMode.Open);
                config = await JsonSerializer.DeserializeAsync<ClientConfig>(filestream);

            }
            ConService.SendConfiguration(config.N, config.L);
            filestream.Close();
        }

        public static int ReadDelay()
        {
            FileStream? filestream;

            try
            {
                filestream = new FileStream("config.json", FileMode.Open);
            }
            catch (Exception ex)
            {
                filestream = new FileStream("config.json", FileMode.Open);
            }
            ClientConfig? config;
            try
            {
                config = JsonSerializer.Deserialize<ClientConfig>(filestream);
            }
            catch (Exception ex)
            {
                filestream.Close();
                filestream = new FileStream("config.json", FileMode.Open);
                config = JsonSerializer.Deserialize<ClientConfig>(filestream);

            }
            filestream.Close();
            return config.Delay;
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
