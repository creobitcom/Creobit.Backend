#if CREOBIT_BACKEND_STEAM && UNITY_STANDALONE
using Steamworks;
using System;
using System.Collections.Generic;

namespace Creobit.Backend.Inventory
{
    public sealed class SteamInventory : ISteamInventory
    {
        #region IInventory

        IEnumerable<IDefinition> IInventory.Definitions => (IEnumerable<IDefinition>)_definitions
            ?? Array.Empty<IDefinition>();

        IEnumerable<IItem> IInventory.Items => (IEnumerable<IItem>)_items
            ?? Array.Empty<IItem>();

        async void IInventory.LoadDefinitions(Action onComplete, Action onFailure)
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
                _definitions = new List<SteamDefinition>();

                foreach (var (DefinitionId, SteamDefId) in DefinitionMap)
                {
                    var inventoryDef = this.FindInventoryDefBySteamDefId(SteamDefId);

                    if (inventoryDef == null)
                    {
                        var exception = new Exception($"The InventoryDef is not found for the DefinitionId \"{DefinitionId}\"!");

                        ExceptionHandler.Process(exception);

                        continue;
                    }

                    _definitions.Add(new SteamDefinition(DefinitionId, inventoryDef));
                }
            }
        }

        async void IInventory.LoadItems(Action onComplete, Action onFailure)
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
                    var definitionId = this.FindDefinitionIdBySteamDefId(inventoryItem.DefId);

                    if (string.IsNullOrWhiteSpace(definitionId))
                    {
                        continue;
                    }

                    var definition = this.FindDefinitionByDefinitionId(definitionId);

                    if (definition == null)
                    {
                        var exception = new Exception($"The Definition is not found for the DefinitionId \"{definitionId}\"!");

                        ExceptionHandler.Process(exception);

                        continue;
                    }

                    var item = new SteamItem(definition, inventoryItem)
                    {
                        Consume = Consume
                    };

                    _items.Add(item);
                }
            }
        }

        #endregion
        #region ISteamInventory

        IEnumerable<(string DefinitionId, int SteamDefId)> ISteamInventory.DefinitionMap => DefinitionMap;

        InventoryDef[] ISteamInventory.InventoryDefs => Steamworks.SteamInventory.Definitions
            ?? Array.Empty<InventoryDef>();

        InventoryItem[] ISteamInventory.InventoryItems => Steamworks.SteamInventory.Items
            ?? Array.Empty<InventoryItem>();

        #endregion
        #region SteamInventory

        private List<SteamDefinition> _definitions;
        private List<SteamItem> _items;

        private IEnumerable<(string DefinitionId, int SteamDefId)> _definitionMap;
        private IExceptionHandler _exceptionHandler;

        public IEnumerable<(string DefinitionId, int SteamDefId)> DefinitionMap
        {
            get => _definitionMap ?? Array.Empty<ValueTuple<string, int>>();
            set => _definitionMap = value;
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
