namespace Generic.Config
{    
    public class ServerConfigurator
    {
        public ServerConfigurator() { }

        public static void GetConfig()
        {

        }

        public static void CreateConfig()
        {

        }
    }

    public record class ClientConfiguration
    {
        public int N { get; set; }
        public int L { get; set; }
    }

    public record class ServerConfiguration
    {
        public string IP { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 8080;
        public string NameDB { get; set; } = "ML START";
        public string ConnectionDBString { get; set; } = "default";
    }
}
