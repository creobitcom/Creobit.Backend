namespace Creobit.Backend
{
    public static class PlayFabStoreExtensions
    {
        #region PlayFabStoreExtensions

        public static string GetCurrencyId(this IPlayFabStore self, string virtualCurrency)
        {
            foreach (var (CurrencyId, VirtualCurrency) in self.CurrencyMap)
            {
                if (VirtualCurrency == virtualCurrency)
                {
                    return CurrencyId;
                }
            }

            return null;
        }

        public static string GetItemId(this IPlayFabStore self, string productId)
        {
            foreach (var (ProductId, ItemId) in self.ProductMap)
            {
                if (ProductId == productId)
                {
                    return ItemId;
                }
            }

            return null;
        }

        public static string GetProductId(this IPlayFabStore self, string itemId)
        {
            foreach (var (ProductId, ItemId) in self.ProductMap)
            {
                if (ItemId == itemId)
                {
                    return ProductId;
                }
            }

            return null;
        }

        public static string GetVirtualCurrency(this IPlayFabStore self, string currencyId)
        {
            foreach (var (CurrencyId, VirtualCurrency) in self.CurrencyMap)
            {
                if (CurrencyId == currencyId)
                {
                    return VirtualCurrency;
                }
            }

            return null;
        }

        #endregion
    }
}