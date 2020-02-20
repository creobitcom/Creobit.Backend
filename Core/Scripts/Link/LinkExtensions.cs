using System;
using System.Threading.Tasks;

namespace Creobit.Backend.Link
{
    public static class LinkExtensions
    {
        #region LinkExtensions

        private const int MillisecondsDelay = 10;

        public static async Task LinkAsync(this ILinkCode self, string linkKey)
        {
            var invokeResult = default(bool?);

            self.Link(linkKey,
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

        public static async Task<(string LinkKey, DateTime LinkKeyExpirationTime)> RequestLinkKeyAsync(this ILinkCode self, int linkKeyLenght)
        {
            var returnResult = default((string LinkKey, DateTime LinkKeyExpirationTime)?);
            var invokeResult = default(bool?);

            self.RequestLinkKey(linkKeyLenght,
                data =>
                {
                    returnResult = data;
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

            return returnResult.Value;
        }

        #endregion
    }
}
