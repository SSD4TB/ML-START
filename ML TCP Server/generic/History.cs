using Generic.LogService;
using Generic.Persons;
using static Serilog.Events.LogEventLevel;

namespace Generic.History
{
    internal class Speaker
    {
        static void Speak()
        {
            Logger.CreateLogDirectory(
                Debug,
                Information,
                Warning,
                Error
                );

            Bank bank = new Bank();
            Person person = new Person("Коротышка");
            Person person1 = new Person("Незнайка");
            Person person2 = new Person("Козлик");
            Person nameMiga = new Person("Мига");
            StackPeoples stackPeoples = new StackPeoples();

            person.BuyStock(bank, stackPeoples);
            person1.SellStock(bank, stackPeoples);
            person2.SellStock(bank, stackPeoples);
            nameMiga.ExchangeMoney(bank);
            while (bank.stocksToSell > 450000)
            {
                bank.SellStockToStack(stackPeoples);
                Console.WriteLine($"=============={stackPeoples.popularity}==============");
            }
            bank.SellAllStocks();

        }

    }
}
