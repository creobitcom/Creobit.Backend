using System;
using System.Threading.Tasks;

namespace Creobit.Backend.Inventory
{
    public static class InventoryExtensions
    {
        #region InventoryExtensions

        private const int MillisecondsDelay = 10;

        public static IItemDefinition FindItemDefinitionByItemDefinitionId(this IInventory self, string itemDefinitionId)
        {
            foreach (var itemDefinition in self.ItemDefinitions)
            {
                if (itemDefinition.Id == itemDefinitionId)
                {
                    return itemDefinition;
                }
            }

            return null;
        }

        public static async Task LoadItemDefinitionsAsync(this IInventory self)
        {
            var invokeResult = default(bool?);

            self.LoadItemDefinitions(
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

        public static async Task LoadItemsAsync(this IInventory self)
        {
            var invokeResult = default(bool?);

            self.LoadItems(
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
