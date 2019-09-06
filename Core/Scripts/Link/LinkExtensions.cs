using System;
using System.Threading.Tasks;

namespace Creobit.Backend.Link
{
    public static class LinkExtensions
    {
        #region LinkExtensions

        private const int MillisecondsDelay = 10;

        public static async Task LinkAsync(this ILink self, string linkKey)
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

        public static async Task<string> RequestLinkKeyAsync(this ILink self, int linkKeyLenght)
        {
            var valueResult = default(string);
            var invokeResult = default(bool?);

            self.RequestLinkKey(linkKeyLenght,
                linkKey =>
                {
                    valueResult = linkKey;
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
