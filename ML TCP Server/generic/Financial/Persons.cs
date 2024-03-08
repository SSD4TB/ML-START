namespace Generic.Persons
{
    #region Bank
    internal class Bank : IFoundOperation
    {
        public Bank(double genericNum)
        {
            this.genericNum = genericNum;
        }

        private double genericNum;
        public int stocksToSell = 2000000;
        readonly Random random = new();

        public string ExchangeMoney(string name)
        {
            return $"{name} обменивает купюры.";
        }

        public string BuyStockByPerson(int count, string name, IPopularityMod popularityMod)
        {
            stocksToSell -= count;
            popularityMod.UpPopularity();
            return $"{name} покупает {count} акций. На продаже осталось акций: {stocksToSell}";
        }

        public string SellStockByPerson(int count, string name, IPopularityMod popularityMod)
        {
            stocksToSell += count;
            popularityMod.UpPopularity();
            return $"{name} продаёт {count} акций. На продаже осталось акций: {stocksToSell}";
        }

        public string SellStockToStack(IPopularityMod popularityMod)
        {
            int timeStock = random.Next(500, 1000) * popularityMod.ReturnLevel();
            stocksToSell -= timeStock;
            popularityMod.UpPopularity();
            return $"Продано акций: {timeStock}. После продажи акций гражданам на счету осталось: {this.stocksToSell}";
        }

        public string SellAllStocks(Company company)
        {
            stocksToSell = 0;
            return $"2.000.000 акций компании {company.Name} были проданы. Курс акций составил {genericNum}$";
        }
    }
    #endregion
    #region Persons
    abstract class Peoples
    {
        public string name;
        public int stocks = 0;
    }
    #region MainCharacters
    class Person : Peoples, IFinancialOperation
    {
        public Person(string name)
        {
            this.name = name;
        }

        private int timeCount;
        readonly Random random = new();

        public string BuyStock(IFoundOperation foundOperation, IPopularityMod popularityMod)
        {
            timeCount = random.Next(100);
            foundOperation.BuyStockByPerson(timeCount, name, popularityMod);
            this.stocks += timeCount;
            foundOperation.SellStockToStack(popularityMod);

            return foundOperation.BuyStockByPerson(timeCount, name, popularityMod) + "=" + foundOperation.SellStockToStack(popularityMod);
        }

        public string ExchangeMoney(IFoundOperation foundOperation)
        {
            return foundOperation.ExchangeMoney(name);
        }

        public string SellStock(IFoundOperation foundOperation, IPopularityMod popularityMod)
        {
            timeCount = random.Next(100);
            return foundOperation.SellStockByPerson(timeCount, name, popularityMod) + "=" + foundOperation.SellStockToStack(popularityMod);
        }
    }
    #endregion

    #region OtherCharacters
    class StackPeoples : Peoples, IPopularityMod
    {


        int mod = 1;
        public int maxPopularity = 0;
        public int popularity = 0;
        readonly Random rnd = new();

        public void UpPopularity()
        {
            // Console.WriteLine("Интерес к акциям со стороны других граждан вырос");
            if (this.maxPopularity == 0)
            {
                this.popularity += rnd.Next(10 * mod, 20 * mod);
            }
            else if (this.maxPopularity == 1) { this.popularity = 500; }
            else
            {
                // Console.WriteLine("[ПОПУЛЯРНОСТЬ]: Нарушена целостность популярности");
            }
            CheckPopularity();

        }
        private void CheckPopularity()
        {
            if (this.popularity >= 50)
            {
                Waiting();
                if (this.popularity >= 100 && this.maxPopularity == 0)
                    MaxPopularity();
            }
        }

        private void Waiting()
        {
            // Console.WriteLine("[ПОПУЛЯРНОСТЬ]: Всё больше людей заинтересовались акциями компании.");
            mod++;
        }

        private void MaxPopularity()
        {
            maxPopularity = 1;
            // Console.WriteLine("[ПОПУЛЯРНОСТЬ]: Большинство людей узнали про акции компании и стремятся их купить, образуя длинные очереди.");
        }

        public int ReturnLevel() => this.popularity;

    }

    record class Company
    {
        public string Name;
        public string HistoryString;

        public Company(string name)
        {
            Name = name;
            HistoryString = $"Компания {Name} выходит на рынок.";
        }
    }
    #endregion
    #endregion
}