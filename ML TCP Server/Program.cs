using Generic.LogService;
using ML_TCP_Server.HostService;
using static Serilog.Events.LogEventLevel;

namespace tcpServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Logger.CreateLogDirectory(
                Information,
                Warning,
                Error
                );
            await HostServer.RunHost();
        }
    }
}