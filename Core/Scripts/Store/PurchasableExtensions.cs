using System;
using System.Threading.Tasks;

namespace Creobit.Backend.Store
{
    public static class PurchasableExtensions
    {
        #region PurchasableExtensions

        private const int MillisecondsDelay = 10;

        public static async Task PurchaseAsync(this IPurchasable self)
        {
            var invokeResult = default(bool?);

            self.Purchase(
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
