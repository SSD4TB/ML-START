using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoggerLibrary;
//using static System.Runtime.InteropServices.JavaScript.JSType;
using static Serilog.Events.LogEventLevel;

namespace ServiceLibrary
{
    internal class Config
    {

        public static void checkFile(string path)
        {

        }

        private static void createConfig()
        {
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(currentDirectory, "config.txt");

            if (!File.Exists(filePath))
            {
                string userConfig = "7 5";
                File.WriteAllText(filePath, userConfig);
            }
        }

        public static List<int> getNumFromConfig()
        {


            return new List<int>();
        }

        public static string getConnectString()
        {
            return "";
        }
        
    }
}
