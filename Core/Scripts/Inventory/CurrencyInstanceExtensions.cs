using System;
using System.Threading.Tasks;

namespace Creobit.Backend.Inventory
{
    public static class CurrencyInstanceExtensions
    {
        #region CurrencyInstanceExtensions

        private const int MillisecondsDelay = 10;

        public static async Task ConsumeAsync(this ICurrencyInstance self, uint count)
        {
            var invokeResult = default(bool?);

            self.Consume(count,
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
