using System;
using System.Threading.Tasks;

namespace Creobit.Backend
{
    public static class ConsumableExtensions
    {
        #region ConsumableExtensions

        private const int MillisecondsDelay = 10;

        public static async Task ConsumeAsync(this IConsumable self, uint count)
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
