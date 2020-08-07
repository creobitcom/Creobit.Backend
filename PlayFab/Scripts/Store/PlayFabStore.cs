#if CREOBIT_BACKEND_PLAYFAB
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Creobit.Backend.Store
{
    public class PlayFabStore : IPlayFabStore
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

                            GetStoreItems();
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

            void GetStoreItems()
            {
                if (string.IsNullOrWhiteSpace(StoreId))
                {
                    UpdateProducts();

                    onComplete();

                    return;
                }

                try
                {
                    PlayFabClientAPI.GetStoreItems(
                        new GetStoreItemsRequest()
                        {
                            CatalogVersion = CatalogVersion,
                            StoreId = StoreId
                        },
                        result =>
                        {
                            StoreItems = result.Store;

                            UpdateProducts();

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
        #region IStore

        IEnumerable<IProduct> IStore.Products => Products;

        IEnumerable<ISubscription> IStore.Subscriptions => Subscriptions;

        #endregion
        #region IPlayFabStore

        string IPlayFabStore.CatalogVersion => CatalogVersion;

        IEnumerable<(string PriceId, string PlayFabVirtualCurrencyId)> IPlayFabStore.PriceMap => PriceMap;

        IEnumerable<(string ProductId, string PlayFabItemId)> IPlayFabStore.ProductMap => ProductMap;

        string IPlayFabStore.StoreId => StoreId;

        #endregion
        #region PlayFabStore

        public readonly string CatalogVersion;
        public readonly string StoreId;

        private IList<CatalogItem> _catalogItems;
        private IList<IProduct> _products;
        private IList<StoreItem> _storeItems;
        private IList<ISubscription> _subscriptions;

        private IEnumerable<(string PriceId, string PlayFabVirtualCurrencyId)> _priceMap;
        private IEnumerable<(string ProductId, string PlayFabItemId)> _productMap;

        private IExceptionHandler _exceptionHandler;
        private IPlayFabErrorHandler _playFabErrorHandler;

        public PlayFabStore(string catalogVersion, string storeId)
        {
            CatalogVersion = string.IsNullOrWhiteSpace(catalogVersion) ? null : catalogVersion;
            StoreId = string.IsNullOrWhiteSpace(storeId) ? null : storeId;
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

        public IPlayFabErrorHandler PlayFabErrorHandler
        {
            get => _playFabErrorHandler ?? Backend.PlayFabErrorHandler.Default;
            set => _playFabErrorHandler = value;
        }

        public IEnumerable<(string PriceId, string PlayFabVirtualCurrencyId)> PriceMap
        {
            get => _priceMap ?? Array.Empty<(string PriceId, string PlayFabVirtualCurrencyId)>();
            set => _priceMap = value;
        }

        public IEnumerable<(string ProductId, string PlayFabItemId)> ProductMap
        {
            get => _productMap ?? Array.Empty<(string ProductId, string PlayFabItemId)>();
            set => _productMap = value;
        }

        private IList<IProduct> Products
        {
            get => _products ?? Array.Empty<IProduct>();
            set => _products = value;
        }

        private IList<StoreItem> StoreItems
        {
            get => _storeItems ?? Array.Empty<StoreItem>();
            set => _storeItems = value;
        }

        private IList<ISubscription> Subscriptions
        {
            get => _subscriptions ?? Array.Empty<ISubscription>();
            set => _subscriptions = value;
        }

        protected virtual void PurchaseViaRealCurrency(IPurchasableItem purchasableItem, Action<string> onComplete, Action onFailure)
        {
            ExceptionHandler.Process(new NotSupportedException());

            onFailure();
        }

        protected virtual void PurchaseViaVirtualCurrency(IPurchasableItem purchasableItem, Action<string> onComplete, Action onFailure)
        {
            var playFabProduct = (IPlayFabProduct)purchasableItem;
            var playFabPrice = (IPlayFabPrice)purchasableItem.Price;
            var catalogItem = playFabProduct.CatalogItem;

            try
            {
                PlayFabClientAPI.PurchaseItem(
                    new PurchaseItemRequest()
                    {
                        CatalogVersion = catalogItem.CatalogVersion,
                        ItemId = catalogItem.ItemId,
                        Price = Convert.ToInt32(playFabPrice.Value),
                        StoreId = string.IsNullOrWhiteSpace(StoreId) ? null : StoreId,
                        VirtualCurrency = playFabPrice.VirtualCurrencyId
                    },
                    result =>
                    {
                        onComplete(string.Empty);
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

        private void UpdateProducts()
        {
            Products = CreateProducts();

            List<IProduct> CreateProducts()
            {
                var products = new List<IProduct>();

                foreach (var (ProductId, PlayFabItemId) in ProductMap)
                {
                    var catalogItem = CatalogItems.FirstOrDefault(x => x.ItemId == PlayFabItemId);

                    if (catalogItem == null)
                    {
                        var exception = new Exception($"The CatalogItem is not found for the PlayFabItemId \"{PlayFabItemId}\"!");

                        ExceptionHandler.Process(exception);

                        continue;
                    }

                    var storeItem = StoreItems.FirstOrDefault(x => x.ItemId == PlayFabItemId);
                    var virtualCurrencyPrices = storeItem?.VirtualCurrencyPrices ?? catalogItem.VirtualCurrencyPrices ?? new Dictionary<string, uint>();

                    if (!virtualCurrencyPrices.Any())
                    {
                        var product = new PlayFabProduct(ProductId, default(IPrice), catalogItem, default(StoreItem));
                        products.Add(product);

                        continue;
                    }

                    foreach (var (PriceId, PlayFabVirtualCurrencyId) in PriceMap)
                    {
                        if (!virtualCurrencyPrices.TryGetValue(PlayFabVirtualCurrencyId, out var value))
                        {
                            continue;
                        }

                        if (PlayFabVirtualCurrencyId == "RM")
                        {
                            var price = new PlayFabPrice(PriceId, "USD", value, PlayFabVirtualCurrencyId);
                            var product = new PlayFabProduct(ProductId, price, catalogItem, storeItem)
                            {
                                PurchaseDelegate = PurchaseViaRealCurrency
                            };

                            products.Add(product);
                        }
                        else
                        {
                            var price = new PlayFabPrice(PriceId, null, value, PlayFabVirtualCurrencyId);
                            var product = new PlayFabProduct(ProductId, price, catalogItem, storeItem)
                            {
                                PurchaseDelegate = PurchaseViaVirtualCurrency
                            };

                            products.Add(product);
                        }
                    }
                }

                return products;
            }
        }

        #endregion
    }
}
#endif
