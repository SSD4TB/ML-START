using ML_TCP_Server.HostService;

namespace tcpServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await HostServer.RunHost();
        }
    }
}