using System;
using System.Threading.Tasks;

namespace Creobit.Backend.User
{
    public static class UserDataExtensions
    {
        #region UserDataExtensions

        private const int MillisecondsDelay = 10;

        public static async Task<T> ReadAsync<T>(this IUserData self)
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

        public static async Task WriteAsync(this IUserData self, object data)
        {
            var invokeResult = default(bool?);

            self.Write(data,
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
