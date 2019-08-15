using System;
using System.Threading.Tasks;

namespace Creobit.Backend
{
    public static class AuthExtensions
    {
        #region AuthExtensions

        private const int MillisecondsDelay = 10;

        public static async Task LoginAsync(this IAuth self)
        {
            var invokeResult = default(bool?);

            self.Login(
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

        public static async Task LogoutAsync(this IAuth self)
        {
            var invokeResult = default(bool?);

            self.Logout(
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