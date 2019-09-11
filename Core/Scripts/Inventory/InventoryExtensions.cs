using System;
using System.Threading.Tasks;

namespace Creobit.Backend.Inventory
{
    public static class InventoryExtensions
    {
        #region InventoryExtensions

        private const int MillisecondsDelay = 10;

        public static TCurrencyDefinition FindCurrencyDefinitionByCurrencyDefinitionId<TCurrencyDefinition, TCurrency, TItemDefinition, TItemInstance>(this IInventory<TCurrencyDefinition, TCurrency, TItemDefinition, TItemInstance> self, string currencyDefinitionId)
            where TCurrencyDefinition : ICurrencyDefinition
            where TCurrency : ICurrencyInstance
            where TItemDefinition : IItemDefinition
            where TItemInstance : IItemInstance
        {
            foreach (var currencyDefinition in self.CurrencyDefinitions)
            {
                if (currencyDefinition.Id == currencyDefinitionId)
                {
                    return currencyDefinition;
                }
            }

            return default;
        }

        public static TItemDefinition FindItemDefinitionByItemDefinitionId<TCurrencyDefinition, TCurrency, TItemDefinition, TItemInstance>(this IInventory<TCurrencyDefinition, TCurrency, TItemDefinition, TItemInstance> self, string itemDefinitionId)
            where TCurrencyDefinition : ICurrencyDefinition
            where TCurrency : ICurrencyInstance
            where TItemDefinition : IItemDefinition
            where TItemInstance : IItemInstance
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

        public static async Task LoadCurrencyDefinitionsAsync(this IInventory self)
        {
            var invokeResult = default(bool?);

            self.LoadCurrencyDefinitions(
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

        public static async Task LoadCurrencyInstancesAsync(this IInventory self)
        {
            var invokeResult = default(bool?);

            self.LoadCurrencyInstances(
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

        public static async Task LoadItemInstancesAsync(this IInventory self)
        {
            var invokeResult = default(bool?);

            self.LoadItemInstances(
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
