using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CSLight
{
    #region Bank
    internal class Bank : IFoundOperation
    {
        public int stocksToSell = 2000000;
        Random random = new Random();

        public void ExchangeMoney(string name)
        {
            Console.WriteLine($"[БИРЖА]: {name} обменивает купюры.");
        }

        public void BuyStockByPerson(int count, string name, IPopularityMod popularityMod)
        {
            Console.WriteLine($"[БИРЖА]: {name} покупает {count} акций.");
            this.stocksToSell -= count;
            Console.WriteLine($"[БИРЖА]: Осталось акций: {this.stocksToSell}");
            popularityMod.UpPopularity();
        }

        public void SellStockByPerson(int count, string name, IPopularityMod popularityMod)
        {
            Console.WriteLine($"[БИРЖА]: {name} продаёт {count} акций.");
            this.stocksToSell += count;
            Console.WriteLine($"[БИРЖА]: Акций стало: {this.stocksToSell}");
            popularityMod.UpPopularity();
        }

        public void SellStockToStack(IPopularityMod popularityMod)
        {
            int timeStock = random.Next(500, 1000) * popularityMod.returnLevel();
            this.stocksToSell -= timeStock;
            Console.WriteLine($"После продажи акций гражданам на счету осталось: {this.stocksToSell}");
            Console.WriteLine($"Продано акций: {timeStock}");
            popularityMod.UpPopularity();
        }

        public void SellAllStocks()
        {
            this.stocksToSell = 0;
            Console.WriteLine("2.000.000 акций компании были проданы.");
        }
    }
    #endregion
    #region Persons
    abstract class Peoples
    {
        public string name;
        public int stocks = 0;
    }

    class Person : Peoples, IFinancialOperation
    {
        public Person(string name)
        {
            this.name = name;
        }

        private int timeCount;
        Random random = new Random();

        public void BuyStock(IFoundOperation foundOperation, IPopularityMod popularityMod)
        {
            timeCount = random.Next(100);
            foundOperation.BuyStockByPerson(timeCount, name, popularityMod);
            this.stocks += timeCount;
            foundOperation.SellStockToStack(popularityMod);
            Thread.Sleep(1000);
        }

        public void ExchangeMoney(IFoundOperation foundOperation)
        {
            foundOperation.ExchangeMoney(name);
            Thread.Sleep(1000);
        }

        public void SellStock(IFoundOperation foundOperation, IPopularityMod popularityMod)
        {
            timeCount = random.Next(100);
            foundOperation.SellStockByPerson(timeCount, name, popularityMod);
            foundOperation.SellStockToStack(popularityMod);
            Thread.Sleep(1000);
        }
    }

    class StackPeoples : Peoples, IPopularityMod
    {


        int mod = 1;
        public int maxPopularity = 0;
        public int popularity = 0;
        Random rnd = new Random();

        public void UpPopularity()
        {
            Console.WriteLine("Интерес к акциям со стороны других граждан вырос");
            if (this.maxPopularity == 0)
            {
                this.popularity += rnd.Next(10 * mod, 20 * mod);
            }
            else if (this.maxPopularity == 1) { this.popularity = 500; }
            else
            {
                Console.WriteLine("[ПОПУЛЯРНОСТЬ]: Нарушена целостность популярности");
            }
            Thread.Sleep(1000);
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
            Console.WriteLine("[ПОПУЛЯРНОСТЬ]: Всё больше людей заинтересовались акциями компании.");
            this.mod++;
        }

        private void MaxPopularity()
        {
            this.maxPopularity = 1;
            Console.WriteLine("[ПОПУЛЯРНОСТЬ]: Большинство людей узнали про акции компании и стремятся их купить, образуя длинные очереди.");
        }

        public int returnLevel() => this.popularity;

    }

    //record class Company
    //{
    //    public string name;

    //    public Company(string name)
    //    {
    //        this.name = name;
    //    }
    //}
    #endregion
}