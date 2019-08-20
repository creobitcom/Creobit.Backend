using System;
using System.Threading.Tasks;

namespace Creobit.Backend
{
    public static class ApplicationDataExtensions
    {
        #region ApplicationDataExtensions

        private const int MillisecondsDelay = 10;

        public static async Task<T> ReadAsync<T>(this IApplicationData self)
            where T : class, new()
        {
            var valueResult = default(T);
            var invokeResult = default(bool?);

            self.Read<T>(
                data =>
                {
                    valueResult = data;
                    invokeResult = true;
                },
                () => invokeResult = false);

            while (!invokeResult.HasValue)
            {
                await Task.Delay(MillisecondsDelay);
            }

            if (!invokeResult.Value)
            {
                throw new InvalidOperationException();
            }

            return valueResult;
        }

        #endregion
    }
}
