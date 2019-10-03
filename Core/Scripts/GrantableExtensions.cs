using System;
using System.Threading.Tasks;

namespace Creobit.Backend
{
    public static class GrantableExtensions
    {
        #region GrantableExtensions

        private const int MillisecondsDelay = 10;

        public static async Task GrantAsync(this IGrantable self, uint count)
        {
            var invokeResult = default(bool?);

            self.Grant(count,
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
