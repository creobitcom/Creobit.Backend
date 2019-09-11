using System;
using System.Threading.Tasks;

namespace Creobit.Backend.Inventory
{
    public static class CurrencyDefinitionExtensions
    {
        #region CurrencyDefinitionExtensions

        private const int MillisecondsDelay = 10;

        public static async Task<TCurrencyInstance> GrantAsync<TCurrencyInstance>(this ICurrencyDefinition<TCurrencyInstance> self, uint count)
            where TCurrencyInstance : ICurrencyInstance
        {
            var invokeResult = default(bool?);
            var currencyInstanceResult = default(TCurrencyInstance);

            self.Grant(count,
                currencyInstance =>
                {
                    invokeResult = true;
                    currencyInstanceResult = currencyInstance;
                },
                () =>
                {
                    invokeResult = false;
                });

            while (!invokeResult.HasValue)
            {
                await Task.Delay(MillisecondsDelay);
            }

            if (!invokeResult.Value)
            {
                throw new InvalidOperationException();
            }

            return currencyInstanceResult;
        }

        #endregion
    }
}
