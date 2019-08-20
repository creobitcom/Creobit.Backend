using System;
using System.Threading.Tasks;

namespace Creobit.Backend
{
    public static class ProductExtensions
    {
        #region ProductExtensions

        private const int MillisecondsDelay = 10;

        public static string GetCurrencyCode(this IProduct self, string currencyId)
        {
            foreach (var (CurrencyId, Price, CurrencyCode) in self.Prices)
            {
                if (CurrencyId == currencyId)
                {
                    return CurrencyCode;
                }
            }

            return null;
        }

        public static int? GetPrice(this IProduct self, string currencyId)
        {
            foreach (var (CurrencyId, Price, CurrencyCode) in self.Prices)
            {
                if (CurrencyId == currencyId)
                {
                    return Convert.ToInt32(Price);
                }
            }

            return null;
        }

        public static async Task PurchaseAsync(this IProduct product, string currencyId)
        {
            var invokeResult = default(bool?);

            product.Purchase(currencyId,
                () => invokeResult = true,
                () => invokeResult = false);

            while (!invokeResult.HasValue)
            {
                await Task.Delay(MillisecondsDelay);
            }

            if (!invokeResult.Value)
            {
                throw new InvalidOperationException();
            }
        }

        #endregion
    }
}
