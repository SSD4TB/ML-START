﻿using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Text;

namespace ML_UI_App.LogService
{
    internal class Logger
    {
        public static void LogByTemplate(LogEventLevel logEventLevel, Exception? ex = null, string note = "")
        {
            StringBuilder info = new();
            info.Append(note);

            if(ex != null)
            {
                info.Append($"; {ex.Source}; {ex.GetType()}; {ex.Message}");
            }

            Log.Write(logEventLevel, info.ToString());
        }

        public static void CreateLogDirectory(params LogEventLevel[] logEventLevels)
        {
            string currentDate = DateTime.Now.ToString("yyyy/MM/dd");

            if (!Directory.Exists("logs") || !Directory.Exists(currentDate))
            {
                Directory.CreateDirectory(@"log\" + currentDate);
            }
            LoggerConfiguration loggerConfig = new();
            string logName = string.Empty;

            foreach (var logEventLevel in logEventLevels)
            {
                logName = logEventLevel.ToString().ToLower() + "Log";
                loggerConfig.WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(evt => evt.Level == logEventLevel)
                .WriteTo.File($@"log\{currentDate}\{logName}.txt"));
            };
            Log.Logger = loggerConfig.CreateLogger();

        }
    }
}
