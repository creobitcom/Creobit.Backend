using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Creobit.Backend.Inventory
{
    public static class ItemDefinitionExtensions
    {
        #region ItemDefinitionExtensions

        private const int MillisecondsDelay = 10;

        public static async Task<IEnumerable<TItemInstance>> GrantAsync<TItemInstance>(this IItemDefinition<TItemInstance> self, uint count)
            where TItemInstance : IItemInstance
        {
            var invokeResult = default(bool?);
            var itemInstancesResult = default(IEnumerable<TItemInstance>);

            self.Grant(count,
                itemInstances =>
                {
                    invokeResult = true;
                    itemInstancesResult = itemInstances;
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

            return itemInstancesResult;
        }

        #endregion
    }
}
