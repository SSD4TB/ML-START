using System.Text.Json;

namespace Generic.Config
{    
    public class ServerConfigurator
    {
        public ServerConfigurator() { }
        //TODO: Сделать ссылку на конфигурацию и сделать учёт конфигурации в хостсервайсе
        public static async Task<ServerConfiguration> GetConfig()
        {
            FileStream filestream;

            try
            {
                filestream = new FileStream("config.json", FileMode.Open);
            }
            catch (Exception ex)
            {
                await CreateConfig();
                filestream = new FileStream("config.json", FileMode.Open);
            }
            ServerConfiguration config;
            try
            {
                config = await JsonSerializer.DeserializeAsync<ServerConfiguration>(filestream);
            }
            catch (Exception ex)
            {
                filestream.Close();
                await CreateConfig();
                filestream = new FileStream("config.json", FileMode.Open);
                config = await JsonSerializer.DeserializeAsync<ServerConfiguration>(filestream);

            }
            filestream.Close();
            return config;
        }

        public static async Task CreateConfig()
        {
            FileStream filestream = new("config.json", FileMode.OpenOrCreate);
            ServerConfiguration conf = new("127.0.0.1", 8080, "ML START", @"Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=""ML START"";Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
            await JsonSerializer.SerializeAsync(filestream, conf);
            Console.WriteLine("Конфигурация восстановлена.");
            filestream.Close();
        }
    }

    public record class ClientConfiguration
    {
        public int N { get; set; }
        public int L { get; set; }
    }

    public record class ServerConfiguration
    {
        public ServerConfiguration(string iP, int port, string nameDB, string connectionDBString)
        {
            IP = iP;
            Port = port;
            NameDB = nameDB;
            ConnectionDBString = connectionDBString;
        }
    
        public string IP { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 8080;
        public string NameDB { get; set; } = "ML START";
        public string ConnectionDBString { get; set; } = "default";
    }
}
