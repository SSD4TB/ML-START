using Generic.LogService;
using static Serilog.Events.LogEventLevel;

namespace Generic.Num
{
    #region Calculate Num
    internal class GenericNum
    {
        public static double GetGenericNum(int n, int l)
        {
            Random rnd = new();

            int[] firstArray = Enumerable.Range(5, 15).Where(x => x % 2 != 0).ToArray();
            double[] secondArray = new double[13];
            double[,] mathArray = new double[8, 13];

            for (int i = 0; i < 13; i++)
            {
                double tempVal = rnd.NextDouble() * (15.0 - (-12.0)) + (-12.0);
                secondArray[i] = tempVal;
            }

            Logger.LogByTemplate(Information, note: "Запущены вычисления особого числа");

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

            double[] firstNumber = Enumerable.Range(0, mathArray.GetLength(1)).Select(col => mathArray[n % 8, col]).ToArray();
            double[] secondNumber = Enumerable.Range(0, mathArray.GetLength(0)).Select(row => mathArray[row, l % 13]).ToArray();

            double checkFirst = firstNumber.Min();
            double checkSecond = secondNumber.Average();

            double answer = (Math.Round((firstNumber.Min() + secondNumber.Average()), 4));

            if (double.IsNaN(answer))
            {
                Logger.LogByTemplate(Warning,
                    note: $"Вычисления привели к ошибке в выходном ответе. Результат = {answer}; Генерация числа запускается заново.");
                return GetGenericNum(n, l);
            }
            return answer;
        }
    }
    #endregion
}