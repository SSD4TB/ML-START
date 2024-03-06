using Serilog;
using Serilog.Events;
using System.Text;

namespace Generic.LogService
{
    internal class Logger
    {
        public static void LogByTemplate(LogEventLevel logEventLevel, Exception? ex = null, string note = "")
        {
            StringBuilder info = new StringBuilder();
            info.Append(note);

            if(ex != null)
            {
                info.Append($"; {ex.Source}; {ex.GetType()}; error message: \"{ex.Message}\"");
            }

            Log.Write(logEventLevel, info.ToString());
        }

        public static void CreateLogDirectory(params LogEventLevel[] logEventLevels)
        {
            string currentDate = DateTime.Now.Date.ToShortDateString();

            if (!Directory.Exists("logs") || !Directory.Exists(currentDate))
            {
                Directory.CreateDirectory(@"log\" + currentDate);
            }
            var loggerConfig = new LoggerConfiguration();
            string logName = string.Empty;

            foreach (var logEventLevel in logEventLevels)
            {
                logName = logEventLevel.ToString().ToLower() + "Log";
                loggerConfig.WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(evt => evt.Level == logEventLevel)
                .WriteTo.File($@"logs\{logName}.txt"));
            };
            Log.Logger = loggerConfig.CreateLogger();

        }
    }
}
