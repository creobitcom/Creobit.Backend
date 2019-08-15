using System;
using System.Threading.Tasks;

namespace Creobit.Backend
{
    public static class UserExtensions
    {
        #region UserExtensions

        private const int MillisecondsDelay = 10;

        public static async Task SetUserNameAsync(this IUser self, string userName)
        {
            var invokeResult = default(bool?);

            self.SetUserName(userName,
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