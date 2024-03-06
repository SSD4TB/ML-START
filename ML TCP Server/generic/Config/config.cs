using System.Text.Json;

namespace Generic.Config
{
    internal class Configurator
    {
        public static async Task CreateFile()
        {
            FileStream fs = new FileStream("config.json", FileMode.OpenOrCreate);
            Configuration conf = new Configuration(7, 5, 2048, 500);
            await JsonSerializer.SerializeAsync<Configuration>(fs, conf);
            Console.WriteLine("������������ �������������.");
            fs.Close();
        }

        static async Task ReadFile()
        {
            FileStream? fs;

            try
            {
                fs = new FileStream("config.json", FileMode.Open);
            }
            catch (Exception ex)
            {
                await CreateFile();
                fs = new FileStream("config.json", FileMode.Open);
            }
            Configuration? config;
            try
            {
                config = await JsonSerializer.DeserializeAsync<Configuration>(fs);
            }
            catch (Exception ex)
            {
                fs.Close();
                await CreateFile();
                fs = new FileStream("config.json", FileMode.Open);
                config = await JsonSerializer.DeserializeAsync<Configuration>(fs);

            }

            Console.WriteLine($"N: {config?.N}  L: {config?.L}  Log: {config?.LogMaxSize}  Delay: {config?.Delay}");
            fs.Close();
        }
    }

    internal class Configuration
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
