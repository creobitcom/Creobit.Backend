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

        IEnumerable<IDefinition> IInventory.Definitions => (IEnumerable<IDefinition>)_definitions
            ?? Array.Empty<IDefinition>();

        IEnumerable<IItem> IInventory.Items => (IEnumerable<IItem>)_items
            ?? Array.Empty<IItem>();

        void IInventory.LoadDefinitions(Action onComplete, Action onFailure)
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
                _definitions = new List<PlayFabDefinition>();

                foreach (var (DefinitionId, PlayFabItemId) in DefinitionMap)
                {
                    var catalogItem = this.FindCatalogItemByPlayFabItemId(PlayFabItemId);

                    if (catalogItem == null)
                    {
                        var exception = new Exception($"The CatalogItem is not found for the DefinitionId \"{DefinitionId}\"!");

                        ExceptionHandler.Process(exception);

                        continue;
                    }

                    _definitions.Add(new PlayFabDefinition(DefinitionId, catalogItem));
                }

                foreach (var definition in _definitions)
                {
                    var catalogItem = definition.CatalogItem;
                    var bundle = catalogItem.Bundle;

                    if (bundle != null && bundle.BundledItems != null)
                    {
                        InitializeBundledDefinitions(definition, bundle.BundledItems);
                    }

                    var container = catalogItem.Container;

                    if (container != null && container.ItemContents != null)
                    {
                        InitializeBundledDefinitions(definition, container.ItemContents);
                    }
                }

                void InitializeBundledDefinitions(PlayFabDefinition definition, IEnumerable<string> bundledPlayFabItemIds)
                {
                    foreach (var bundledPlayFabItemId in bundledPlayFabItemIds)
                    {
                        var bundledDefinitionId = this.FindDefinitionIdByPlayFabItemId(bundledPlayFabItemId);

                        if (string.IsNullOrWhiteSpace(bundledDefinitionId))
                        {
                            continue;
                        }

                        var bundledDefinition = this.FindDefinitionByDefinitionId(bundledDefinitionId);

                        if (bundledDefinition == null)
                        {
                            var exception = new Exception($"The Definition is not found for the DefinitionId \"{bundledDefinitionId}\"!");

                            ExceptionHandler.Process(exception);

                            continue;
                        }

                        definition.AddBundledDefinition(bundledDefinition, 1);
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
                    var definitionId = this.FindDefinitionIdByPlayFabItemId(itemInstance.ItemId);

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

                    var item = new PlayFabItem(definition, itemInstance)
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

        IEnumerable<(string DefinitionId, string PlayFabItemId)> IPlayFabInventory.DefinitionMap => DefinitionMap;

        GetCatalogItemsResult IPlayFabInventory.GetCatalogItemsResult => _getCatalogItemsResult;

        GetUserInventoryResult IPlayFabInventory.GetUserInventoryResult => _getUserInventoryResult;

        #endregion
        #region PlayFabInventory

        private readonly string CatalogVersion;

        private List<PlayFabDefinition> _definitions;
        private List<PlayFabItem> _items;

        private GetCatalogItemsResult _getCatalogItemsResult;
        private GetUserInventoryResult _getUserInventoryResult;

        private IEnumerable<(string DefinitionId, string PlayFabItemId)> _definitionMap;
        private IExceptionHandler _exceptionHandler;
        private IPlayFabErrorHandler _playFabErrorHandler;

        public PlayFabInventory(string catalogVersion)
        {
            CatalogVersion = catalogVersion;
        }

        public IEnumerable<(string DefinitionId, string PlayFabItemId)> DefinitionMap
        {
            get => _definitionMap ?? Array.Empty<ValueTuple<string, string>>();
            set => _definitionMap = value;
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
