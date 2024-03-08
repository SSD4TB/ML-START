namespace Generic.Persons
{
    interface IFinancialOperation
    {
        string BuyStock(IFoundOperation foundOperation, IPopularityMod popularityMod);
        string SellStock(IFoundOperation foundOperation, IPopularityMod popularityMod);
        string ExchangeMoney(IFoundOperation foundOperation);
    }

    interface IPopularityMod
    {
       void UpPopularity();
       int ReturnLevel();
    }

    interface IFoundOperation
    {
        string BuyStockByPerson(int count, string name, IPopularityMod popularityMod);
        string SellStockByPerson(int count, string name, IPopularityMod popularityMod);
        string SellStockToStack(IPopularityMod popularityMod);
        string ExchangeMoney(string name);
    }
}

