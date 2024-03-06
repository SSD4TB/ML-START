using Serilog;
using Serilog.Events;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Generic.LogService;

namespace Generic.Config
{
    internal class Configurator
    {
        public static void GetConfiguration()
        {
            
        }
        public async Task CreateConfiguration()
        {
            FileStream filestream = new("config.json", FileMode.OpenOrCreate);
            Configuration config = new(7, 5, 2048, 500);
            await JsonSerializer.SerializeAsync<Configuration>(filestream, config);
        }
    }

    internal class Configuration
    {
        public Configuration(int FirstNum, int SecondNum, int LogSize, int DelayValue)
        {
            FirstNumN = FirstNum;
            SecondNumL = SecondNum;
            LogMaxSize = LogSize;
            Delay = DelayValue;
        }
        [JsonPropertyName("N")]
        public int FirstNumN { get; set; }
        [JsonPropertyName("L")]
        public int SecondNumL { get; set; }
        public int LogMaxSize { get; set; }
        public int Delay {  get; set; }
    }
}
