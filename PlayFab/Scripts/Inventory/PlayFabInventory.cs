#if CREOBIT_BACKEND_PLAYFAB
using Creobit.Backend.User;
using Creobit.Backend.Wallet;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Creobit.Backend.Inventory
{
    public sealed class PlayFabInventory : IPlayFabInventory
    {
        #region IRefreshable

        void IRefreshable.Refresh(Action onComplete, Action onFailure)
        {
            GetCatalogItems();

            void GetCatalogItems()
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
                            CatalogItems = result.Catalog;

                            GetUserInventory();
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

            void GetUserInventory()
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
                            var userInventory = infoResultPayload.UserInventory;

                            ItemInstances = userInventory.ToDictionary(x => x.ItemInstanceId);

                            UpdateItems();

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
        }

        #endregion
        #region IInventory

        IEnumerable<IItem> IInventory.Items => Items;

        #endregion
        #region IPlayFabInventory

        string IPlayFabInventory.CatalogVersion => CatalogVersion;

        IEnumerable<(string ItemId, string PlayFabItemId)> IPlayFabInventory.ItemMap => ItemMap;

        IPlayFabUser IPlayFabInventory.User => User;

        IPlayFabWallet IPlayFabInventory.Wallet => Wallet;

        #endregion
        #region PlayFabInventory

        private readonly string CatalogVersion;
        private readonly IPlayFabUser User;
        private readonly IPlayFabWallet Wallet;

        private IList<CatalogItem> _catalogItems;
        private IDictionary<string, ItemInstance> _itemInstances;
        private IList<IItem> _items;

        private IExceptionHandler _exceptionHandler;
        private IEnumerable<(string ItemId, string PlayFabItemId)> _itemMap;
        private IPlayFabErrorHandler _playFabErrorHandler;

        public PlayFabInventory(string catalogVersion, IPlayFabUser user, IPlayFabWallet wallet)
        {
            CatalogVersion = string.IsNullOrWhiteSpace(catalogVersion) ? null : catalogVersion;
            User = user;
            Wallet = wallet;
        }

        private IList<CatalogItem> CatalogItems
        {
            get => _catalogItems ?? Array.Empty<CatalogItem>();
            set => _catalogItems = value;
        }

        public IExceptionHandler ExceptionHandler
        {
            get => _exceptionHandler ?? Backend.ExceptionHandler.Default;
            set => _exceptionHandler = value;
        }

        private IDictionary<string, ItemInstance> ItemInstances
        {
            get => _itemInstances ?? new ReadOnlyDictionary<string, ItemInstance>(new Dictionary<string, ItemInstance>());
            set => _itemInstances = value;
        }

        public IEnumerable<(string ItemId, string PlayFabItemId)> ItemMap
        {
            get => _itemMap ?? Array.Empty<(string ItemId, string PlayFabItemId)>();
            set => _itemMap = value;
        }

        private IList<IItem> Items
        {
            get => _items ?? Array.Empty<IItem>();
            set => _items = value;
        }

        public IPlayFabErrorHandler PlayFabErrorHandler
        {
            get => _playFabErrorHandler ?? Backend.PlayFabErrorHandler.Default;
            set => _playFabErrorHandler = value;
        }

        private void Consume(IItem item, uint count, Action onComplete, Action onFailure)
        {
            var playFabItemId = this.FindPlayFabItemId(item.Id);

            if (playFabItemId == null)
            {
                var exception = new Exception($"The PlayFabItemId is not found for the ItemId \"{item.Id}\"!");

                ExceptionHandler.Process(exception);

                onFailure();

                return;
            }


            try
            {
                var itemInstance = ((PlayFabItem)item).ItemInstance;

                PlayFabClientAPI.ConsumeItem(
                    new ConsumeItemRequest()
                    {
                        ConsumeCount = Convert.ToInt32(count),
                        ItemInstanceId = itemInstance.ItemInstanceId
                    },
                    result =>
                    {
                        itemInstance.RemainingUses = result.RemainingUses;

                        UpdatePlayFabItems(new ItemInstance[] { itemInstance });

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

        private void Grant(IItem item, uint count, Action onComplete, Action onFailure)
        {
#if ENABLE_PLAYFABADMIN_API
            var playFabItemId = this.FindPlayFabItemId(item.Id);

            if (playFabItemId == null)
            {
                var exception = new Exception($"The PlayFabItemId is not found for the ItemId \"{item.Id}\"!");

                ExceptionHandler.Process(exception);

                onFailure();

                return;
            }

            try
            {
                PlayFabAdminAPI.GrantItemsToUsers(
                    new PlayFab.AdminModels.GrantItemsToUsersRequest()
                    {
                        CatalogVersion = CatalogVersion,
                        ItemGrants = new List<PlayFab.AdminModels.ItemGrant>()
                        {
                            new PlayFab.AdminModels.ItemGrant()
                            {
                                ItemId = playFabItemId,
                                PlayFabId = User.Id
                            }
                        }
                    },
                    result =>
                    {
                        var itemInstances = result.ItemGrantResults
                            .Select(
                                x =>
                                {
                                    return new ItemInstance()
                                    {
                                        Annotation = x.Annotation,
                                        BundleContents = x.BundleContents,
                                        BundleParent = x.BundleParent,
                                        CatalogVersion = x.CatalogVersion,
                                        CustomData = x.CustomData,
                                        DisplayName = x.DisplayName,
                                        Expiration = x.Expiration,
                                        ItemClass = x.ItemClass,
                                        ItemId = x.ItemId,
                                        ItemInstanceId = x.ItemInstanceId,
                                        PurchaseDate = x.PurchaseDate,
                                        RemainingUses = x.RemainingUses,
                                        UnitCurrency = x.UnitCurrency,
                                        UnitPrice = x.UnitPrice,
                                        UsesIncrementedBy = x.UsesIncrementedBy
                                    };
                                });

                        UpdatePlayFabItems(itemInstances);

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
#else
            ExceptionHandler.Process(new NotSupportedException());

            onFailure();
#endif
        }

        internal void UpdateItems()
        {
            Items = CreateItems();

            List<IItem> CreateItems()
            {
                var items = new List<IItem>();

                foreach (var (ItemId, PlayFabItemId) in ItemMap)
                {
                    var catalogItem = CatalogItems.FirstOrDefault(x => x.ItemId == PlayFabItemId);

                    if (catalogItem == null)
                    {
                        var exception = new Exception($"The CatalogItem is not found for the PlayFabItemId \"{PlayFabItemId}\"!");

                        ExceptionHandler.Process(exception);

                        continue;
                    }

                    var bundledCurrencies = CreateBundledCurrencies(catalogItem);
                    var bundledItems = CreateBundledItems(catalogItem);
                    var itemInstances = ItemInstances.Values
                        .Where(x => x.ItemId == PlayFabItemId);

                    if (itemInstances.Any())
                    {
                        foreach (var itemInstance in itemInstances)
                        {
                            var item = items
                                .Cast<PlayFabItem>()
                                .FirstOrDefault(x => x.ItemInstance == itemInstance) as PlayFabItem;

                            if (item == null)
                            {
                                item = items
                                    .FirstOrDefault(x => x.Id == ItemId && ((IPlayFabItem)x).ItemInstance == null) as PlayFabItem;
                            }

                            if (item == null)
                            {
                                item = new PlayFabItem(ItemId);
                            }

                            item.BundledCurrencies = bundledCurrencies;
                            item.BundledItems = bundledItems;
                            item.CatalogItem = catalogItem;
                            item.ConsumeDelegate = Consume;
                            item.Count = itemInstance.RemainingUses ?? 1;
                            item.GrantDelegate = Grant;
                            item.ItemInstance = itemInstance;

                            items.Add(item);
                        }
                    }
                    else
                    {
                        var item = items
                            .FirstOrDefault(x => x.Id == ItemId) as PlayFabItem ?? new PlayFabItem(ItemId);

                        item.BundledCurrencies = bundledCurrencies;
                        item.BundledItems = bundledItems;
                        item.CatalogItem = catalogItem;
                        item.ConsumeDelegate = null;
                        item.Count = 0;
                        item.GrantDelegate = Grant;
                        item.ItemInstance = null;

                        items.Add(item);
                    }
                }

                return items;
            }

            List<ICurrency> CreateBundledCurrencies(CatalogItem catalogItem)
            {
                var bundledCurrencies = new List<ICurrency>();

                if (catalogItem.Bundle != null)
                {
                    var bundle = catalogItem.Bundle;

                    if (bundle.BundledVirtualCurrencies != null)
                    {
                        Cache(bundle.BundledVirtualCurrencies);
                    }
                }

                if (catalogItem.Container != null)
                {
                    var container = catalogItem.Container;

                    if (container.VirtualCurrencyContents != null)
                    {
                        Cache(container.VirtualCurrencyContents);
                    }
                }

                return bundledCurrencies;

                void Cache(IDictionary<string, uint> virtualCurrency)
                {
                    foreach (var (CurrencyId, PlayFabVirtualCurrencyId) in Wallet.CurrencyMap)
                    {
                        if (PlayFabVirtualCurrencyId == "RM")
                        {
                            continue;
                        }

                        if (!virtualCurrency.TryGetValue(PlayFabVirtualCurrencyId, out var count))
                        {
                            continue;
                        }

                        var currency = new Wallet.Currency(CurrencyId)
                        {
                            Count = Convert.ToInt32(count)
                        };

                        bundledCurrencies.Add(currency);
                    }
                }
            }

            List<IItem> CreateBundledItems(CatalogItem catalogItem)
            {
                var bundledItems = new List<IItem>();

                if (catalogItem.Bundle != null)
                {
                    var bundle = catalogItem.Bundle;

                    if (bundle.BundledItems != null)
                    {
                        Cache(bundle.BundledItems);
                    }
                }

                if (catalogItem.Container != null)
                {
                    var container = catalogItem.Container;

                    if (container.ItemContents != null)
                    {
                        Cache(container.ItemContents);
                    }
                }

                return bundledItems;

                void Cache(List<string> itemIds)
                {
                    var itemIdGroups = itemIds
                        .GroupBy(x => x)
                        .ToDictionary(x => x.Key, y => y.Count());

                    foreach (var (ItemId, PlayFabItemId) in ItemMap)
                    {
                        if (!itemIdGroups.TryGetValue(PlayFabItemId, out var count))
                        {
                            continue;
                        }

                        var item = new PlayFabItem(ItemId)
                        {
                            CatalogItem = catalogItem,
                            Count = count
                        };

                        bundledItems.Add(item);
                    }
                }
            }
        }

        internal void UpdatePlayFabItems(IEnumerable<ItemInstance> itemInstances)
        {
            foreach (var itemInstance in itemInstances)
            {
                if (ItemInstances.TryGetValue(itemInstance.ItemInstanceId, out var currentItemInstances))
                {
                    currentItemInstances.RemainingUses = itemInstance.RemainingUses;
                }
                else
                {
                    ItemInstances[itemInstance.ItemInstanceId] = itemInstance;
                }
            }

            UpdateItems();
        }

        #endregion
    }
}
#endif
