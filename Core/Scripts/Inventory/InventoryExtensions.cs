using System;
using System.Threading.Tasks;

namespace Creobit.Backend.Inventory
{
    public static class InventoryExtensions
    {
        #region InventoryExtensions

        private const int MillisecondsDelay = 10;

        public static TItemDefinition FindItemDefinitionByItemDefinitionId<TItemDefinition, TItem>(this IInventory<TItemDefinition, TItem> self, string itemDefinitionId)
            where TItem : IItem<TItemDefinition>
            where TItemDefinition : IItemDefinition
        {
            foreach (var itemDefinition in self.ItemDefinitions)
            {
                if (itemDefinition.Id == itemDefinitionId)
                {
                    return itemDefinition;
                }
            }

            return default;
        }

        public static async Task LoadItemDefinitionsAsync<TItem, TItemDefinition>(this IInventory<TItemDefinition, TItem> self)
            where TItem : IItem<TItemDefinition>
            where TItemDefinition : IItemDefinition
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

        public static async Task LoadItemsAsync<TItem, TItemDefinition>(this IInventory<TItemDefinition, TItem> self)
            where TItem : IItem<TItemDefinition>
            where TItemDefinition : IItemDefinition
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
