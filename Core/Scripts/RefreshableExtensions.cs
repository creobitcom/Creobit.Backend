using System;
using System.Threading.Tasks;

namespace Creobit.Backend
{
    public static class RefreshableExtensions
    {
        #region RefreshableExtensions

        private const int MillisecondsDelay = 10;

        public static async Task RefreshAsync(this IRefreshable self)
        {
            var invokeResult = default(bool?);

            self.Refresh(
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
