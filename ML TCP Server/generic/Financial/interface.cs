namespace Generic.Persons
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
       int ReturnLevel();
    }

    interface IFoundOperation
    {
        void BuyStockByPerson(int count, string name, IPopularityMod popularityMod);
        void SellStockByPerson(int count, string name, IPopularityMod popularityMod);
        void SellStockToStack(IPopularityMod popularityMod);
        void ExchangeMoney(string name);
    }
}

