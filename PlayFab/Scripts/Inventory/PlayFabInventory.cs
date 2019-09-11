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

        void IInventory.LoadCurrencyDefinitions(Action onComplete, Action onFailure)
        {
            try
            {
                PlayFabClientAPI.GetPlayerCombinedInfo(
                    new GetPlayerCombinedInfoRequest()
                    {
                        InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
                        {
                            GetUserVirtualCurrency = true
                        }
                    },
                    result =>
                    {
                        var infoResultPayload = result.InfoResultPayload;

                        if (_getPlayerCombinedInfoResultPayload == null)
                        {
                            _getPlayerCombinedInfoResultPayload = infoResultPayload;
                        }
                        else
                        {
                            _getPlayerCombinedInfoResultPayload.UserVirtualCurrency = infoResultPayload.UserVirtualCurrency;
                            _getPlayerCombinedInfoResultPayload.UserVirtualCurrencyRechargeTimes = infoResultPayload.UserVirtualCurrencyRechargeTimes;
                        }

                        CreateCurrencyDefinitions();

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

            void CreateCurrencyDefinitions()
            {
                var userVirtualCurrency = _getPlayerCombinedInfoResultPayload.UserVirtualCurrency;
                var userVirtualCurrencyRechargeTimes = _getPlayerCombinedInfoResultPayload.UserVirtualCurrencyRechargeTimes;

                _currencyDefinitions = new List<PlayFabCurrencyDefinition>();

                foreach (var (CurrencyDefinitionId, PlayFabVirtualCurrencyId) in CurrencyDefinitionMap)
                {
                    if (PlayFabVirtualCurrencyId == "RM")
                    {
                        continue;
                    }

                    if (!userVirtualCurrency.ContainsKey(PlayFabVirtualCurrencyId))
                    {
                        var exception = new Exception($"Invalid entry in the CurrencyDefinitionMap \"({CurrencyDefinitionId}, {PlayFabVirtualCurrencyId})\"!");

                        ExceptionHandler.Process(exception);

                        continue;
                    }

                    userVirtualCurrencyRechargeTimes.TryGetValue(PlayFabVirtualCurrencyId, out var virtualCurrencyRechargeTime);

                    var currencyDefinition = new PlayFabCurrencyDefinition(CurrencyDefinitionId, virtualCurrencyRechargeTime)
                    {
                        Grant = Grant
                    };

                    _currencyDefinitions.Add(currencyDefinition);
                }
            }
        }

        void IInventory.LoadCurrencyInstances(Action onComplete, Action onFailure)
        {
            try
            {
                PlayFabClientAPI.GetPlayerCombinedInfo(
                    new GetPlayerCombinedInfoRequest()
                    {
                        InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
                        {
                            GetUserVirtualCurrency = true
                        }
                    },
                    result =>
                    {
                        var infoResultPayload = result.InfoResultPayload;

                        if (_getPlayerCombinedInfoResultPayload == null)
                        {
                            _getPlayerCombinedInfoResultPayload = infoResultPayload;
                        }
                        else
                        {
                            _getPlayerCombinedInfoResultPayload.UserVirtualCurrency = infoResultPayload.UserVirtualCurrency;
                            _getPlayerCombinedInfoResultPayload.UserVirtualCurrencyRechargeTimes = infoResultPayload.UserVirtualCurrencyRechargeTimes;
                        }

                        CreateCurrencyInstances();

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

            void CreateCurrencyInstances()
            {
                _currencyInstances = new List<PlayFabCurrencyInstance>();

                foreach (var entry in _getPlayerCombinedInfoResultPayload.UserVirtualCurrency)
                {
                    var currencyDefinitionId = this.FindCurrencyDefinitionIdByPlayFabVirtualCurrencyId(entry.Key);

                    if (string.IsNullOrWhiteSpace(currencyDefinitionId))
                    {
                        continue;
                    }

                    var currencyDefinition = this.FindCurrencyDefinitionByCurrencyDefinitionId(currencyDefinitionId);

                    if (currencyDefinition == null)
                    {
                        var exception = new Exception($"The CurrencyDefinition is not found for the СurrencyDefinitionId \"{currencyDefinitionId}\"!");

                        ExceptionHandler.Process(exception);

                        continue;
                    }

                    var currencyInstance = new PlayFabCurrencyInstance(currencyDefinition, Convert.ToUInt32(entry.Value))
                    {
                        Consume = Consume
                    };

                    _currencyInstances.Add(currencyInstance);
                }
            }
        }

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

                        CreateItemDefinitions();

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

            void CreateItemDefinitions()
            {
                _itemDefinitions = new List<PlayFabItemDefinition>();

                foreach (var (ItemDefinitionId, PlayFabItemId) in ItemDefinitionMap)
                {
                    var catalogItem = this.FindCatalogItemByPlayFabItemId(PlayFabItemId);

                    if (catalogItem == null)
                    {
                        var exception = new Exception($"Invalid entry in the ItemDefinitionMap \"({ItemDefinitionId}, {PlayFabItemId})\"!");

                        ExceptionHandler.Process(exception);

                        continue;
                    }

                    var itemDefinition = new PlayFabItemDefinition(ItemDefinitionId, catalogItem)
                    {
                        Grant = Grant
                    };

                    _itemDefinitions.Add(itemDefinition);
                }

                foreach (var itemDefinition in _itemDefinitions)
                {
                    var catalogItem = itemDefinition.CatalogItem;
                    var bundle = catalogItem.Bundle;

                    if (bundle != null && bundle.BundledItems != null)
                    {
                        if (bundle.BundledVirtualCurrencies != null)
                        {
                            InitializeBundledCurrencyDefinitions(itemDefinition, bundle.BundledVirtualCurrencies);
                        }

                        if (bundle.BundledItems != null)
                        {
                            InitializeBundledItemDefinitions(itemDefinition, bundle.BundledItems);
                        }
                    }

                    var container = catalogItem.Container;

                    if (container != null && container.ItemContents != null)
                    {
                        if (container.VirtualCurrencyContents != null)
                        {
                            InitializeBundledCurrencyDefinitions(itemDefinition, container.VirtualCurrencyContents);
                        }

                        if (container.ItemContents != null)
                        {
                            InitializeBundledItemDefinitions(itemDefinition, container.ItemContents);
                        }
                    }
                }

                void InitializeBundledCurrencyDefinitions(PlayFabItemDefinition itemDefinition, Dictionary<string, uint> bundledPlayFabVirtualCurrencies)
                {
                    foreach (var bundledPlayFabVirtualCurrency in bundledPlayFabVirtualCurrencies)
                    {
                        var currencyDefinitionId = this.FindCurrencyDefinitionIdByPlayFabVirtualCurrencyId(bundledPlayFabVirtualCurrency.Key);

                        if (string.IsNullOrWhiteSpace(currencyDefinitionId))
                        {
                            continue;
                        }

                        var currencyDefinition = this.FindCurrencyDefinitionByCurrencyDefinitionId(currencyDefinitionId);

                        if (currencyDefinition == null)
                        {
                            var exception = new Exception($"The CurrencyDefinition is not found for the CurrencyDefinitionId \"{currencyDefinitionId}\"!");

                            ExceptionHandler.Process(exception);

                            continue;
                        }

                        itemDefinition.AddBundledCurrencyDefinition(currencyDefinition, bundledPlayFabVirtualCurrency.Value);
                    }
                }

                void InitializeBundledItemDefinitions(PlayFabItemDefinition itemDefinition, IEnumerable<string> bundledPlayFabItemIds)
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

        void IInventory.LoadItemInstances(Action onComplete, Action onFailure)
        {
            try
            {
                PlayFabClientAPI.GetPlayerCombinedInfo(
                    new GetPlayerCombinedInfoRequest()
                    {
                        InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
                        {
                            GetUserInventory = true
                        }
                    },
                    result =>
                    {
                        var infoResultPayload = result.InfoResultPayload;

                        if (_getPlayerCombinedInfoResultPayload == null)
                        {
                            _getPlayerCombinedInfoResultPayload = infoResultPayload;
                        }
                        else
                        {
                            _getPlayerCombinedInfoResultPayload.UserInventory = infoResultPayload.UserInventory;
                        }

                        CreateItemInstances();

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

            void CreateItemInstances()
            {
                _itemInstances = new List<PlayFabItemInstance>();

                foreach (var entry in _getPlayerCombinedInfoResultPayload.UserInventory)
                {
                    var itemDefinitionId = this.FindItemDefinitionIdByPlayFabItemId(entry.ItemId);

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

                    var itemInstance = new PlayFabItemInstance(itemDefinition, entry)
                    {
                        Consume = Consume
                    };

                    _itemInstances.Add(itemInstance);
                }
            }
        }

        #endregion
        #region IInventory<IPlayFabCurrencyDefinition, IPlayFabCurrencyInstance, IPlayFabItemDefinition, IPlayFabItemInstance>

        IEnumerable<IPlayFabCurrencyDefinition> IInventory<IPlayFabCurrencyDefinition, IPlayFabCurrencyInstance, IPlayFabItemDefinition, IPlayFabItemInstance>.CurrencyDefinitions
            => (IEnumerable<IPlayFabCurrencyDefinition>)_currencyDefinitions
            ?? Array.Empty<IPlayFabCurrencyDefinition>();

        IEnumerable<IPlayFabCurrencyInstance> IInventory<IPlayFabCurrencyDefinition, IPlayFabCurrencyInstance, IPlayFabItemDefinition, IPlayFabItemInstance>.CurrencyInstances
            => (IEnumerable<IPlayFabCurrencyInstance>)_currencyInstances
            ?? Array.Empty<IPlayFabCurrencyInstance>();

        IEnumerable<IPlayFabItemDefinition> IInventory<IPlayFabCurrencyDefinition, IPlayFabCurrencyInstance, IPlayFabItemDefinition, IPlayFabItemInstance>.ItemDefinitions
            => (IEnumerable<IPlayFabItemDefinition>)_itemDefinitions
            ?? Array.Empty<IPlayFabItemDefinition>();

        IEnumerable<IPlayFabItemInstance> IInventory<IPlayFabCurrencyDefinition, IPlayFabCurrencyInstance, IPlayFabItemDefinition, IPlayFabItemInstance>.ItemInstances
            => (IEnumerable<IPlayFabItemInstance>)_itemInstances
            ?? Array.Empty<IPlayFabItemInstance>();

        #endregion
        #region IPlayFabInventory

        string IPlayFabInventory.CatalogVersion => CatalogVersion;

        IEnumerable<(string CurrencyDefinitionId, string PlayFabVirtualCurrencyId)> IPlayFabInventory.CurrencyDefinitionMap => CurrencyDefinitionMap;

        IEnumerable<(string ItemDefinitionId, string PlayFabItemId)> IPlayFabInventory.ItemDefinitionMap => ItemDefinitionMap;

        GetCatalogItemsResult IPlayFabInventory.NativeGetCatalogItemsResult => _getCatalogItemsResult;

        GetPlayerCombinedInfoResultPayload IPlayFabInventory.NativeGetPlayerCombinedInfoResultPayload => _getPlayerCombinedInfoResultPayload;

        #endregion
        #region PlayFabInventory

        private readonly string CatalogVersion;

        private List<PlayFabCurrencyDefinition> _currencyDefinitions;
        private List<PlayFabCurrencyInstance> _currencyInstances;
        private List<PlayFabItemDefinition> _itemDefinitions;
        private List<PlayFabItemInstance> _itemInstances;

        private GetCatalogItemsResult _getCatalogItemsResult;
        private GetPlayerCombinedInfoResultPayload _getPlayerCombinedInfoResultPayload;

        private IEnumerable<(string CurrencyDefinitionId, string PlayFabVirtualCurrencyId)> _currencyDefinitionMap;
        private IEnumerable<(string ItemDefinitionId, string PlayFabItemId)> _itemDefinitionMap;

        private IExceptionHandler _exceptionHandler;
        private IPlayFabErrorHandler _playFabErrorHandler;

        public PlayFabInventory(string catalogVersion)
        {
            CatalogVersion = catalogVersion;
        }

        public IEnumerable<(string CurrencyDefinitionId, string PlayFabVirtualCurrencyId)> CurrencyDefinitionMap
        {
            get => _currencyDefinitionMap ?? Array.Empty<ValueTuple<string, string>>();
            set => _currencyDefinitionMap = value;
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

        private void Consume(PlayFabCurrencyInstance currencyInstance, uint count, Action onComplete, Action onFailure)
        {
            ExceptionHandler.Process(new NotSupportedException());

            onFailure();
        }

        private void Consume(PlayFabItemInstance itemInstance, uint count, Action onComplete, Action onFailure)
        {
            try
            {
                var nativeItemInstance = itemInstance.ItemInstance;

                PlayFabClientAPI.ConsumeItem(
                    new ConsumeItemRequest()
                    {
                        ConsumeCount = Convert.ToInt32(count),
                        ItemInstanceId = nativeItemInstance.ItemInstanceId
                    },
                    result =>
                    {
                        nativeItemInstance.RemainingUses = result.RemainingUses;

                        if (nativeItemInstance.RemainingUses == 0)
                        {
                            _itemInstances.Remove(itemInstance);
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

        private void Grant(PlayFabCurrencyDefinition currencyDefinition, uint count, Action<PlayFabCurrencyInstance> onComplete, Action onFailure)
        {
            ExceptionHandler.Process(new NotSupportedException());

            onFailure();
        }

        private void Grant(PlayFabItemDefinition itemDefinition, uint count, Action<IEnumerable<PlayFabItemInstance>> onComplete, Action onFailure)
        {
            ExceptionHandler.Process(new NotSupportedException());

            onFailure();
        }

        #endregion
    }
}
#endif
