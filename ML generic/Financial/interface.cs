using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSLight
{
    interface IFinancialOperation
    {
        void BuyStock(IFoundOperation foundOperation, IPopularityMod popularityMod);
        void SellStock(IFoundOperation foundOperation, IPopularityMod popularityMod);
        void ExchangeMoney(IFoundOperation foundOperation);
    }

    interface IPopularityMod
    {
        void UpPopularity();
       int returnLevel();
    }

    interface IFoundOperation
    {
        void BuyStockByPerson(int count, string name, IPopularityMod popularityMod);
        void SellStockByPerson(int count, string name, IPopularityMod popularityMod);
        void SellStockToStack(IPopularityMod popularityMod);
        void ExchangeMoney(string name);
    }
}

