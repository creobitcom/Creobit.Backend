#if CREOBIT_BACKEND_PLAYFAB
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;

namespace Creobit.Backend.Inventory
{
    public sealed class PlayFabInventory : IPlayFabInventory
    {
        #region IInventory

        IEnumerable<IItemDefinition> IInventory.ItemDefinitions => (IEnumerable<IItemDefinition>)_itemDefinitions ?? Array.Empty<IItemDefinition>();

        IEnumerable<IItem> IInventory.Items => (IEnumerable<IItem>)_items ?? Array.Empty<IItem>();

        void IInventory.LoadItemDefinitions(Action onComplete, Action onFailure)
        {
            try
            {
                PlayFabClientAPI.GetCatalogItems(
                    new GetCatalogItemsRequest()
                    {
                        CatalogVersion = CatalogVersion
                    },
                    result =>
                    {
                        _getCatalogItemsResult = result;

                        CreateDefinitions();

                        onComplete();
                    },
                    error =>
                    {
                        PlayFabErrorHandler.Process(error);

                        onFailure();
                    });
            }
            catch (Exception exception)
            {
                ExceptionHandler.Process(exception);

                onFailure();
            }

            void CreateDefinitions()
            {
                _itemDefinitions = new List<PlayFabItemDefinition>();

                foreach (var (ItemDefinitionId, PlayFabItemId) in ItemDefinitionMap)
                {
                    var catalogItem = this.FindCatalogItemByPlayFabItemId(PlayFabItemId);

                    if (catalogItem == null)
                    {
                        var exception = new Exception($"The CatalogItem is not found for the ItemDefinitionId \"{ItemDefinitionId}\"!");

                        ExceptionHandler.Process(exception);

                        continue;
                    }

                    _itemDefinitions.Add(new PlayFabItemDefinition(ItemDefinitionId, catalogItem));
                }

                foreach (var itemDefinition in _itemDefinitions)
                {
                    var catalogItem = itemDefinition.CatalogItem;
                    var bundle = catalogItem.Bundle;

                    if (bundle != null && bundle.BundledItems != null)
                    {
                        InitializeBundledDefinitions(itemDefinition, bundle.BundledItems);
                    }

                    var container = catalogItem.Container;

                    if (container != null && container.ItemContents != null)
                    {
                        InitializeBundledDefinitions(itemDefinition, container.ItemContents);
                    }
                }

                void InitializeBundledDefinitions(PlayFabItemDefinition itemDefinition, IEnumerable<string> bundledPlayFabItemIds)
                {
                    foreach (var bundledPlayFabItemId in bundledPlayFabItemIds)
                    {
                        var bundledItemDefinitionId = this.FindItemDefinitionIdByPlayFabItemId(bundledPlayFabItemId);

                        if (string.IsNullOrWhiteSpace(bundledItemDefinitionId))
                        {
                            continue;
                        }

                        var bundledItemDefinition = this.FindItemDefinitionByItemDefinitionId(bundledItemDefinitionId);

                        if (bundledItemDefinition == null)
                        {
                            var exception = new Exception($"The ItemDefinition is not found for the ItemDefinitionId \"{bundledItemDefinitionId}\"!");

                            ExceptionHandler.Process(exception);

                            continue;
                        }

                        itemDefinition.AddBundledItemDefinition(bundledItemDefinition, 1);
                    }
                }
            }
        }

        void IInventory.LoadItems(Action onComplete, Action onFailure)
        {
            try
            {
                PlayFabClientAPI.GetUserInventory(
                    new GetUserInventoryRequest()
                    {
                    },
                    result =>
                    {
                        _getUserInventoryResult = result;

                        CreateItems();

                        onComplete();
                    },
                    error =>
                    {
                        PlayFabErrorHandler.Process(error);

                        onFailure();
                    });
            }
            catch (Exception exception)
            {
                ExceptionHandler.Process(exception);

                onFailure();
            }

            void CreateItems()
            {
                _items = new List<PlayFabItem>();

                foreach (var itemInstance in _getUserInventoryResult.Inventory)
                {
                    var itemDefinitionId = this.FindItemDefinitionIdByPlayFabItemId(itemInstance.ItemId);

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

                    var item = new PlayFabItem(itemDefinition, itemInstance)
                    {
                        Consume = Consume
                    };

                    _items.Add(item);
                }
            }
        }

        #endregion
        #region IPlayFabInventory

        string IPlayFabInventory.CatalogVersion => CatalogVersion;

        GetCatalogItemsResult IPlayFabInventory.GetCatalogItemsResult => _getCatalogItemsResult;

        GetUserInventoryResult IPlayFabInventory.GetUserInventoryResult => _getUserInventoryResult;

        IEnumerable<(string ItemDefinitionId, string PlayFabItemId)> IPlayFabInventory.ItemDefinitionMap => ItemDefinitionMap;

        #endregion
        #region PlayFabInventory

        private readonly string CatalogVersion;

        private List<PlayFabItemDefinition> _itemDefinitions;
        private List<PlayFabItem> _items;

        private GetCatalogItemsResult _getCatalogItemsResult;
        private GetUserInventoryResult _getUserInventoryResult;

        private IEnumerable<(string ItemDefinitionId, string PlayFabItemId)> _itemDefinitionMap;
        private IExceptionHandler _exceptionHandler;
        private IPlayFabErrorHandler _playFabErrorHandler;

        public PlayFabInventory(string catalogVersion)
        {
            CatalogVersion = catalogVersion;
        }

        public IEnumerable<(string ItemDefinitionId, string PlayFabItemId)> ItemDefinitionMap
        {
            get => _itemDefinitionMap ?? Array.Empty<ValueTuple<string, string>>();
            set => _itemDefinitionMap = value;
        }

        public IExceptionHandler ExceptionHandler
        {
            get => _exceptionHandler ?? Backend.ExceptionHandler.Default;
            set => _exceptionHandler = value;
        }

        public IPlayFabErrorHandler PlayFabErrorHandler
        {
            get => _playFabErrorHandler ?? Backend.PlayFabErrorHandler.Default;
            set => _playFabErrorHandler = value;
        }

        private void Consume(PlayFabItem playFabItem, int count, Action onComplete, Action onFailure)
        {
            try
            {
                var itemInstance = playFabItem.ItemInstance;

                PlayFabClientAPI.ConsumeItem(
                    new ConsumeItemRequest()
                    {
                        ConsumeCount = count,
                        ItemInstanceId = itemInstance.ItemInstanceId
                    },
                    result =>
                    {
                        itemInstance.RemainingUses = result.RemainingUses;

                        if (itemInstance.RemainingUses == 0)
                        {
                            _items.Remove(playFabItem);
                        }

                        onComplete();
                    },
                    error =>
                    {
                        PlayFabErrorHandler.Process(error);

                        onFailure();
                    });
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
