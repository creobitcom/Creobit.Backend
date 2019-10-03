using System;
using System.Threading.Tasks;

namespace Creobit.Backend.StoreLegacy
{
    public static class StoreExtensions
    {
        #region StoreExtensions

        private const int MillisecondsDelay = 10;

        public static async Task LoadProductsAsync(this IStore self)
        {
            var invokeResult = default(bool?);

            self.LoadProducts(
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
