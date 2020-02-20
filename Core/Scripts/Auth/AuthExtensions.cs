using System;
using System.Threading.Tasks;

namespace Creobit.Backend.Auth
{
    public static class AuthExtensions
    {
        #region AuthExtensions

        private const int MillisecondsDelay = 10;

        /// <summary>
        /// An extension method that allows backward compatibility.
        /// </summary>
        public static void Login(this IAuth self, Action onComplete, Action onFailure)
        {
            self.Login(true, onComplete, onFailure);
        }

        public static async Task LoginAsync(this IAuth self, bool doCreateAccount = true)
        {
            var invokeResult = default(bool?);

            self.Login
            (
                doCreateAccount,
                () => invokeResult = true,
                () => invokeResult = false
            );

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
