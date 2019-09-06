using System;
using System.Threading.Tasks;

namespace Creobit.Backend.User
{
    public static class UserExtensions
    {
        #region UserExtensions

        private const int MillisecondsDelay = 10;

        public static async Task RefreshAsync(this IUser self)
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

        public static async Task SetAvatarUrlAsync(this IUser self, string avatarUrl)
        {
            var invokeResult = default(bool?);

            self.SetAvatarUrl(avatarUrl,
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

        public static async Task SetNameAsync(this IUser self, string name)
        {
            var invokeResult = default(bool?);

            self.SetName(name,
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
