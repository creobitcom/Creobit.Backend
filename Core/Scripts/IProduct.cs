using System;
using System.Collections.Generic;

namespace Creobit.Backend
{
    public interface IProduct
    {
        #region IProduct

        IEnumerable<(string CurrencyId, uint Count)> BundledCurrencies
        {
            get;
        }

        IEnumerable<(IProduct Product, uint Count)> BundledProducts
        {
            get;
        }

        string Description
        {
            get;
        }

        string Id
        {
            get;
        }

        string Name
        {
            get;
        }

        IEnumerable<(string CurrencyId, uint Price, string CurrencyCode)> Prices
        {
            get;
        }

        void Purchase(string currencyId, Action onComplete, Action onFailure);

        #endregion
    }
}
