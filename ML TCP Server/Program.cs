using Generic.LogService;
using ML_TCP_Server.HostService;
using TCPServer.Authorizathion;
using Serilog.Events;

namespace tcpServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Logger.CreateLogDirectory(
                LogEventLevel.Information,
                LogEventLevel.Warning,
                LogEventLevel.Error
                );
            await HostServer.RunHost();
        }
    }
}