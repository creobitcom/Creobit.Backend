#if CREOBIT_BACKEND_PLAYFAB
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;

namespace Creobit.Backend.StoreLegacy
{
    public sealed class PlayFabProduct : IProduct
    {
        #region IProduct

        IEnumerable<(string CurrencyId, uint Count)> IProduct.BundledCurrencies => BundledCurrencies;

        IEnumerable<(IProduct Product, uint Count)> IProduct.BundledProducts => BundledProducts;

        string IProduct.Description => CatalogItem.Description;

        string IProduct.Id => Id;

        string IProduct.Name => CatalogItem.DisplayName;

        IEnumerable<(string CurrencyId, uint Price, string CurrencyCode)> IProduct.Prices => Prices;

        void IProduct.Purchase(string currencyId, Action onComplete, Action onFailure) => Purchase(this, currencyId, onComplete, onFailure);

        #endregion
        #region PlayFabProduct

        private readonly List<(string CurrencyId, uint Count)> BundledCurrencies = new List<(string CurrencyId, uint Count)>();
        private readonly List<(IProduct Product, uint Count)> BundledProducts = new List<(IProduct Product, uint Count)>();
        private readonly string Id;
        private readonly List<(string CurrencyId, uint Price, string CurrencyCode)> Prices = new List<(string CurrencyId, uint Price, string CurrencyCode)>();

        public readonly CatalogItem CatalogItem;
        public readonly StoreItem StoreItem;

        public PlayFabProduct(string id, CatalogItem catalogItem, StoreItem storeItem)
        {
            Id = id;
            CatalogItem = catalogItem;
            StoreItem = storeItem;
        }

        public Action<IProduct, string, Action, Action> Purchase
        {
            get;
            set;
        }

        public void AddBundledCurrency(string currencyId, uint count)
        {
            for (var i = 0; i < BundledCurrencies.Count; ++i)
            {
                if (BundledCurrencies[i].CurrencyId == currencyId)
                {
                    BundledCurrencies[i] = (currencyId, BundledCurrencies[i].Count + count);

                    return;
                }
            }

            BundledCurrencies.Add((currencyId, count));
        }

        public void AddBundledProducts(IProduct product, uint count)
        {
            for (var i = 0; i < BundledProducts.Count; ++i)
            {
                if (BundledProducts[i].Product == product)
                {
                    BundledProducts[i] = (product, BundledProducts[i].Count + count);

                    return;
                }
            }

            BundledProducts.Add((product, count));
        }

        public void SetPrice(string currencyId, uint price, string currencyCode)
        {
            for (var i = 0; i < Prices.Count; ++i)
            {
                if (Prices[i].CurrencyId == currencyId)
                {
                    Prices[i] = (currencyId, price, currencyCode);

                    return;
                }
            }

            Prices.Add((currencyId, price, currencyCode));
        }

        #endregion
    }
}
#endif
