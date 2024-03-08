using Generic.LogService;
using Generic.Num;
using Generic.Persons;
using static Serilog.Events.LogEventLevel;

namespace Generic.History
{
    internal class History
    {
        public static string[] Speak(int n, int l)
        {
            //Logger.CreateLogDirectory(
            //    Debug,
            //    Information,
            //    Warning,
            //    Error
            //    );

            double course = GenericNum.GetGenericNum(n, l);
            while (course < 0)
            {
                course = GenericNum.GetGenericNum(n, l);
            }

            Company company = new("Компания");
            Bank bank = new(course);
            Person person = new("Коротышка");
            Person person1 = new("Незнайка");
            Person person2 = new("Козлик");
            Person nameMiga = new("Мига");
            StackPeoples stackPeoples = new();

            string tempString = company.HistoryString + "=" + person.BuyStock(bank, stackPeoples)
                + "=" + person1.SellStock(bank, stackPeoples) + "=" 
                + person2.SellStock(bank, stackPeoples) + "=" + nameMiga.ExchangeMoney(bank);
            while (bank.stocksToSell > 450000)
            {
                tempString += "=" + bank.SellStockToStack(stackPeoples);
            }
            tempString += "=" + bank.SellAllStocks(company);

            return tempString.Split("=");
        }
    }
}
