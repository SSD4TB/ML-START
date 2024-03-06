using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection.Metadata.Ecma335;
using static Serilog.Events.LogEventLevel;

namespace Generic.Num
{
    internal class GenericNum
    {
        public static double GetGenericNum()
        {
            Random rnd = new Random();

            int[] firstArray = Enumerable.Range(5, 15).Where(x => x % 2 != 0).ToArray();
            double[] secondArray = new double[13];
            double[,] mathArray = new double[8, 13];

            for (int i = 0; i < 13; i++)
            {
                double tempVal = rnd.NextDouble() * (15.0 - (-12.0)) + (-12.0);
                secondArray[i] = tempVal;
                Logger.LogByTemplate(Debug, note: $"X array index {i} = {secondArray[i]} ");
            }

            Logger.LogByTemplate(Information, note: "Application started ");

            for (int i = 0; i < firstArray.Length; i++)
            {
                if (firstArray[i] == 9)
                {
                    for (int j = 0; j < mathArray.GetLength(1); j++)
                    {
                        mathArray[i, j] = Math.Sin(Math.Sin(Math.Pow((secondArray[j] / (secondArray[j] + 0.5)), secondArray[j])));
                    }
                }

                else if (firstArray[i] == 5 || firstArray[i] == 7 || firstArray[i] == 11 || firstArray[i] == 15)
                {
                    for (int j = 0; j < mathArray.GetLength(1); j++)
                    {
                        mathArray[i, j] = Math.Pow((0.5 / (Math.Tan(2 * secondArray[j]) + (2.0 / 3.0))), Math.Cbrt(Math.Cbrt(secondArray[j])));
                    }
                }

                else
                {
                    for (int j = 0; j < mathArray.GetLength(1); j++)
                    {
                        mathArray[i, j] = Math.Tan(Math.Pow(((Math.Pow(Math.E, 1 - secondArray[j] / Math.PI) / 3) / 4), 3));
                    }
                }
            }

            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(currentDirectory, "config.txt");
            Logger.LogByTemplate(Debug,
                note: "Checking and configuring file ");
            Logger.LogByTemplate(Information,
                note: $"Config file path: {filePath}");

            if (!File.Exists(filePath))
            {
                Logger.LogByTemplate(Debug,
                    note: "Config file not found, creating with default content ");
                string userConfig = "7 5";
                File.WriteAllText(filePath, userConfig);
            }

            string fileContent = File.ReadAllText(filePath);
            int n, l;
            try
            {
                n = int.Parse(fileContent.Split()[0]);
                l = int.Parse(fileContent.Split()[1]);

                double[] firstNumber = Enumerable.Range(0, mathArray.GetLength(1)).Select(col => mathArray[n % 8, col]).ToArray();
                double[] secondNumber = Enumerable.Range(0, mathArray.GetLength(0)).Select(row => mathArray[row, l % 13]).ToArray();

                double checkFirst = firstNumber.Min();
                double checkSecond = secondNumber.Average();

                var answer = (Math.Round((firstNumber.Min() + secondNumber.Average()), 4));

                if (double.IsNaN(answer))
                {
                    Logger.LogByTemplate(Warning,
                        note: $"The calculated result is not a valid number. answer = {answer} REPEAT.");
                    return GetGenericNum();
                }
                return answer;
            }
            catch (Exception ex)
            {
                Logger.LogByTemplate(Error,
                    ex, $"Parsing failed. Invalid format in config file.");
                return GetGenericNum();
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}