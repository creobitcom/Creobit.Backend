#if CREOBIT_BACKEND_STEAM && UNITY_STANDALONE
using Steamworks;
using System;
using System.Collections.Generic;

namespace Creobit.Backend.Inventory
{
    public sealed class SteamInventory : ISteamInventory
    {
        #region IInventory

        IEnumerable<ISteamItem> IInventory<ISteamItemDefinition, ISteamItem>.Items
            => (IEnumerable<ISteamItem>)_items
            ?? Array.Empty<ISteamItem>();

        IEnumerable<ISteamItemDefinition> IInventory<ISteamItemDefinition, ISteamItem>.ItemDefinitions
            => (IEnumerable<ISteamItemDefinition>)_itemDefinitions
            ?? Array.Empty<ISteamItemDefinition>();

        async void IInventory<ISteamItemDefinition, ISteamItem>.LoadItems(Action onComplete, Action onFailure)
        {
            try
            {
                var inventoryResult = await Steamworks.SteamInventory.GetAllItemsAsync();

                if (inventoryResult.HasValue)
                {
                    CreateItems();

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

            void CreateItems()
            {
                _items = new List<SteamItem>();

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

                    var item = new SteamItem(itemDefinition, inventoryItem)
                    {
                        Consume = Consume
                    };

                    _items.Add(item);
                }
            }
        }

        async void IInventory<ISteamItemDefinition, ISteamItem>.LoadItemDefinitions(Action onComplete, Action onFailure)
        {
            try
            {
                var result = await Steamworks.SteamInventory.WaitForDefinitions();

                if (result)
                {
                    CreateDefinitions();

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

            void CreateDefinitions()
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

                    _itemDefinitions.Add(new SteamItemDefinition(ItemDefinitionId, inventoryDef));
                }
            }
        }

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
        private List<SteamItem> _items;

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

        private async void Consume(SteamItem steamItem, int count, Action onComplete, Action onFailure)
        {
            try
            {
                var inventoryItem = steamItem.InventoryItem;
                var inventoryResult = await inventoryItem.ConsumeAsync(count);

                if (inventoryResult.HasValue)
                {
                    if (inventoryResult.Value.ItemCount == 0)
                    {
                        _items.Remove(steamItem);
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

        #endregion
    }
}
#endif
