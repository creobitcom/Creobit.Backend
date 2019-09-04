using System;
using System.Threading.Tasks;

namespace Creobit.Backend.Inventory
{
    public static class InventoryExtensions
    {
        #region InventoryExtensions

        private const int MillisecondsDelay = 10;

        public static IDefinition FindDefinitionByDefinitionId(this IInventory self, string definitionId)
        {
            foreach (var definition in self.Definitions)
            {
                if (definition.Id == definitionId)
                {
                    return definition;
                }
            }

            return null;
        }

        public static async Task LoadDefinitions(this IInventory self)
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
