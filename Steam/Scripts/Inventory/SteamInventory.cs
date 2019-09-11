#if CREOBIT_BACKEND_STEAM && UNITY_STANDALONE
using Steamworks;
using System;
using System.Collections.Generic;

namespace Creobit.Backend.Inventory
{
    public sealed class SteamInventory : ISteamInventory
    {
        #region IInventory

        void IInventory.LoadCurrencyDefinitions(Action onComplete, Action onFailure)
        {
            ExceptionHandler.Process(new NotSupportedException());

            onFailure();
        }

        void IInventory.LoadCurrencyInstances(Action onComplete, Action onFailure)
        {
            ExceptionHandler.Process(new NotSupportedException());

            onFailure();
        }

        async void IInventory.LoadItemDefinitions(Action onComplete, Action onFailure)
        {
            try
            {
                var result = await Steamworks.SteamInventory.WaitForDefinitions();

                if (result)
                {
                    CreateItemDefinitions();

                    onComplete();
                }
                else
                {
                    onFailure();
                }
            }
            catch (Exception exception)
            {
                ExceptionHandler.Process(exception);

                onFailure();
            }

            void CreateItemDefinitions()
            {
                _itemDefinitions = new List<SteamItemDefinition>();

                foreach (var (ItemDefinitionId, SteamDefId) in ItemDefinitionMap)
                {
                    var inventoryDef = this.FindInventoryDefBySteamDefId(SteamDefId);

                    if (inventoryDef == null)
                    {
                        var exception = new Exception($"The InventoryDef is not found for the ItemDefinitionId \"{ItemDefinitionId}\"!");

                        ExceptionHandler.Process(exception);

                        continue;
                    }

                    var itemDefinition = new SteamItemDefinition(ItemDefinitionId, inventoryDef)
                    {
                        Grant = Grant
                    };

                    _itemDefinitions.Add(new SteamItemDefinition(ItemDefinitionId, inventoryDef));
                }
            }
        }

        async void IInventory.LoadItemInstances(Action onComplete, Action onFailure)
        {
            try
            {
                var inventoryResult = await Steamworks.SteamInventory.GetAllItemsAsync();

                if (inventoryResult.HasValue)
                {
                    CreateItemInstances();

                    onComplete();
                }
                else
                {
                    onFailure();
                }
            }
            catch (Exception exception)
            {
                ExceptionHandler.Process(exception);

                onFailure();
            }

            void CreateItemInstances()
            {
                _itemInstances = new List<SteamItemInstance>();

                foreach (var inventoryItem in Steamworks.SteamInventory.Items)
                {
                    var itemDefinitionId = this.FindItemDefinitionIdBySteamDefId(inventoryItem.DefId);

                    if (string.IsNullOrWhiteSpace(itemDefinitionId))
                    {
                        continue;
                    }

                    var itemDefinition = this.FindItemDefinitionByItemDefinitionId(itemDefinitionId);

                    if (itemDefinition == null)
                    {
                        var exception = new Exception($"The ItemDefinition is not found for the ItemDefinitionId \"{itemDefinitionId}\"!");

                        ExceptionHandler.Process(exception);

                        continue;
                    }

                    var itemInstance = new SteamItemInstance(itemDefinition, inventoryItem)
                    {
                        Consume = Consume
                    };

                    _itemInstances.Add(itemInstance);
                }
            }
        }

        #endregion
        #region IInventory<ICurrencyDefinition, ICurrencyInstance<ICurrencyDefinition>, ISteamItemDefinition, ISteamItemInstance>

        IEnumerable<ICurrencyDefinition> IInventory<ICurrencyDefinition, ICurrencyInstance<ICurrencyDefinition>, ISteamItemDefinition, ISteamItemInstance>.CurrencyDefinitions
            => Array.Empty<ICurrencyDefinition>();

        IEnumerable<ICurrencyInstance<ICurrencyDefinition>> IInventory<ICurrencyDefinition, ICurrencyInstance<ICurrencyDefinition>, ISteamItemDefinition, ISteamItemInstance>.CurrencyInstances
            => Array.Empty<ICurrencyInstance<ICurrencyDefinition>>();

        IEnumerable<ISteamItemInstance> IInventory<ICurrencyDefinition, ICurrencyInstance<ICurrencyDefinition>, ISteamItemDefinition, ISteamItemInstance>.ItemInstances
            => (IEnumerable<ISteamItemInstance>)_itemInstances
            ?? Array.Empty<ISteamItemInstance>();

        IEnumerable<ISteamItemDefinition> IInventory<ICurrencyDefinition, ICurrencyInstance<ICurrencyDefinition>, ISteamItemDefinition, ISteamItemInstance>.ItemDefinitions
            => (IEnumerable<ISteamItemDefinition>)_itemDefinitions
            ?? Array.Empty<ISteamItemDefinition>();

        #endregion
        #region ISteamInventory

        IEnumerable<(string ItemDefinitionId, int SteamDefId)> ISteamInventory.ItemDefinitionMap => ItemDefinitionMap;

        InventoryDef[] ISteamInventory.InventoryDefs
            => Steamworks.SteamInventory.Definitions
            ?? Array.Empty<InventoryDef>();

        InventoryItem[] ISteamInventory.InventoryItems
            => Steamworks.SteamInventory.Items
            ?? Array.Empty<InventoryItem>();

        #endregion
        #region SteamInventory

        private List<SteamItemDefinition> _itemDefinitions;
        private List<SteamItemInstance> _itemInstances;

        private IEnumerable<(string ItemDefinitionId, int SteamDefId)> _itemDefinitionMap;
        private IExceptionHandler _exceptionHandler;

        public IEnumerable<(string ItemDefinitionId, int SteamDefId)> ItemDefinitionMap
        {
            get => _itemDefinitionMap ?? Array.Empty<ValueTuple<string, int>>();
            set => _itemDefinitionMap = value;
        }

        public IExceptionHandler ExceptionHandler
        {
            get => _exceptionHandler ?? Backend.ExceptionHandler.Default;
            set => _exceptionHandler = value;
        }

        private async void Consume(SteamItemInstance itemInstance, uint count, Action onComplete, Action onFailure)
        {
            try
            {
                var inventoryItem = itemInstance.InventoryItem;
                var inventoryResult = await inventoryItem.ConsumeAsync(Convert.ToInt32(count));

                if (inventoryResult.HasValue)
                {
                    if (inventoryResult.Value.ItemCount == 0)
                    {
                        _itemInstances.Remove(itemInstance);
                    }

                    onComplete();
                }
                else
                {
                    onFailure();
                }
            }
            catch (Exception exception)
            {
                ExceptionHandler.Process(exception);

                onFailure();
            }
        }

        private void Grant(SteamItemDefinition itemDefinition, uint count, Action<IEnumerable<SteamItemInstance>> onComplete, Action onFailure)
        {
            ExceptionHandler.Process(new NotSupportedException());

            onFailure();
        }

        #endregion
    }
}
#endif
