#if CREOBIT_BACKEND_PLAYFAB
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Creobit.Backend
{
    public sealed class PlayFabStore : IPlayFabStore
    {
        #region IStore

        IEnumerable<IProduct> IStore.Products => Products;

        void IStore.LoadProducts(Action onComplete, Action onFailure)
        {
            var errorCount = 0;
            var invokeCount = 2;

            if (_getCatalogItemsResult == null)
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
                            invokeCount -= 1;

                            _getCatalogItemsResult = result;

                            Handle();
                        },
                        error =>
                        {
                            errorCount += 1;
                            invokeCount -= 1;

                            PlayFabErrorHandler?.Process(error);

                            Handle();
                        });
                }
                catch (Exception exception)
                {
                    ExceptionHandler?.Process(exception);

                    onFailure();
                }
            }
            else
            {
                invokeCount -= 1;

                Handle();
            }

            if (_getStoreItemsResult == null && !string.IsNullOrWhiteSpace(StoreId))
            {
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
                            invokeCount -= 1;

                            _getStoreItemsResult = result;

                            Handle();
                        },
                        error =>
                        {
                            errorCount += 1;
                            invokeCount -= 1;

                            PlayFabErrorHandler?.Process(error);

                            Handle();
                        });
                }
                catch (Exception exception)
                {
                    ExceptionHandler?.Process(exception);

                    onFailure();
                }
            }
            else
            {
                invokeCount -= 1;

                Handle();
            }

            void Handle()
            {
                if (invokeCount != 0)
                {
                    return;
                }

                if (errorCount > 0)
                {
                    onFailure();
                }
                else
                {
                    CreateProducts();

                    onComplete();
                }
            }
        }

        #endregion
        #region IPlayFabStore

        string IPlayFabStore.CatalogVersion => CatalogVersion;

        string IPlayFabStore.StoreId => StoreId;

        IEnumerable<(string CurrencyId, string VirtualCurrency)> IPlayFabStore.CurrencyMap => CurrencyMap;

        IEnumerable<(string ProductId, string ItemId)> IPlayFabStore.ProductMap => ProductMap;

        #endregion
        #region PlayFabStore

        private readonly string CatalogVersion;
        private readonly string StoreId;
        private readonly List<PlayFabProduct> Products = new List<PlayFabProduct>();

        private GetCatalogItemsResult _getCatalogItemsResult;
        private GetStoreItemsResult _getStoreItemsResult;

        public PlayFabStore(string catalogVersion, string storeId)
        {
            CatalogVersion = string.IsNullOrWhiteSpace(catalogVersion) ? null : catalogVersion;
            StoreId = string.IsNullOrWhiteSpace(storeId) ? null : storeId;
        }

        public IExceptionHandler ExceptionHandler
        {
            get;
            set;
        } = Backend.ExceptionHandler.Default;

        public IPlayFabErrorHandler PlayFabErrorHandler
        {
            get;
            set;
        } = Backend.PlayFabErrorHandler.Default;

        public IEnumerable<(string CurrencyId, string VirtualCurrency)> CurrencyMap
        {
            get;
            set;
        } = Array.Empty<ValueTuple<string, string>>();

        public IEnumerable<(string ProductId, string ItemId)> ProductMap
        {
            get;
            set;
        } = Array.Empty<ValueTuple<string, string>>();

        private void CreateProducts()
        {
            Products.Clear();

            foreach (var (ProductId, ItemId) in ProductMap)
            {
                var catalogItem = FindCatalogItem(ItemId);

                if (catalogItem == null)
                {
                    Debug.LogError($"CatalogItem is not found for the ProductId \"{ProductId}\"!");

                    continue;
                }

                var storeItem = FindStoreItem(ItemId);
                var product = new PlayFabProduct(ProductId, catalogItem, storeItem)
                {
                    Purchase = Purchase
                };

                Products.Add(product);
            }

            foreach (var product in Products)
            {
                var catalogItem = product.CatalogItem;
                var bundle = catalogItem.Bundle;

                if (bundle != null)
                {
                    if (bundle.BundledVirtualCurrencies != null)
                    {
                        InitializeBundledCurrencies(product, bundle.BundledVirtualCurrencies);
                    }

                    if (bundle.BundledItems != null)
                    {
                        InitializeBundledProducts(product, bundle.BundledItems);
                    }
                }

                var container = catalogItem.Container;

                if (container != null)
                {
                    if (container.VirtualCurrencyContents != null)
                    {
                        InitializeBundledCurrencies(product, container.VirtualCurrencyContents);
                    }

                    if (container.ItemContents != null)
                    {
                        InitializeBundledProducts(product, container.ItemContents);
                    }
                }

                var storeItem = product.StoreItem;

                InitializeProductPrices(product, storeItem == null ? catalogItem.VirtualCurrencyPrices : storeItem.VirtualCurrencyPrices);
            }

            IProduct FindProduct(string productId)
            {
                foreach (IProduct product in Products)
                {
                    if (product.Id == productId)
                    {
                        return product;
                    }
                }

                return null;
            }

            void InitializeBundledCurrencies(PlayFabProduct product, Dictionary<string, uint> bundledVirtualCurrencies)
            {
                foreach (var bundledVirtualCurrency in bundledVirtualCurrencies)
                {
                    var currencyId = this.GetCurrencyId(bundledVirtualCurrency.Key);

                    if (string.IsNullOrWhiteSpace(currencyId))
                    {
                        continue;
                    }

                    product.AddBundledCurrency(currencyId, bundledVirtualCurrency.Value);
                }
            }

            void InitializeBundledProducts(PlayFabProduct product, IEnumerable<string> bundledItemIds)
            {
                foreach (var bundledItemId in bundledItemIds)
                {
                    var bundledProductId = this.GetProductId(bundledItemId);

                    if (string.IsNullOrWhiteSpace(bundledProductId))
                    {
                        continue;
                    }

                    var bundledProduct = FindProduct(bundledProductId);

                    if (bundledProduct == null)
                    {
                        continue;
                    }

                    product.AddBundledProducts(bundledProduct, 1);
                }
            }

            void InitializeProductPrices(PlayFabProduct product, Dictionary<string, uint> virtualCurrencyPrices)
            {
                foreach (var virtualCurrencyPrice in virtualCurrencyPrices)
                {
                    var currencyId = this.GetCurrencyId(virtualCurrencyPrice.Key);

                    if (string.IsNullOrWhiteSpace(currencyId))
                    {
                        Debug.LogError($"CurrencyId is not found for the VirtualCurrency \"{virtualCurrencyPrice.Key}\"!");

                        continue;
                    }

                    var currencyCode = virtualCurrencyPrice.Key == "RM" ? "USD" : string.Empty;

                    product.SetPrice(currencyId, virtualCurrencyPrice.Value, currencyCode);
                }
            }
        }

        public CatalogItem FindCatalogItem(string itemId)
        {
            if (_getCatalogItemsResult == null)
            {
                return null;
            }

            foreach (var catalogItem in _getCatalogItemsResult.Catalog)
            {
                if (catalogItem.ItemId == itemId)
                {
                    return catalogItem;
                }
            }

            return null;
        }

        public StoreItem FindStoreItem(string itemId)
        {
            if (_getStoreItemsResult == null)
            {
                return null;
            }

            foreach (var storeItem in _getStoreItemsResult.Store)
            {
                if (storeItem.ItemId == itemId)
                {
                    return storeItem;
                }
            }

            return null;
        }

        private void Purchase(IProduct product, string currencyId, Action onComplete, Action onFailure)
        {
            var virtualCurrency = this.GetVirtualCurrency(currencyId);

            if (virtualCurrency == null)
            {
                var exception = new Exception($"The VirtualCurrency is not found for the CurrencyId \"{currencyId}\"!");

                ExceptionHandler?.Process(exception);

                onFailure();

                return;
            }

            if (virtualCurrency == "RM")
            {
                var excetion = new NotSupportedException("Real Currency is not supported!");

                ExceptionHandler?.Process(excetion);

                onFailure();
            }
            else
            {
                var itemId = this.GetItemId(product.Id);
                var price = product.GetPrice(currencyId) ?? 0;

                try
                {
                    PlayFabClientAPI.PurchaseItem(
                        new PurchaseItemRequest()
                        {
                            CatalogVersion = CatalogVersion,
                            ItemId = itemId,
                            Price = Convert.ToInt32(price),
                            StoreId = string.IsNullOrWhiteSpace(StoreId) ? null : StoreId,
                            VirtualCurrency = virtualCurrency
                        },
                        result =>
                        {
                            onComplete();
                        },
                        error =>
                        {
                            PlayFabErrorHandler?.Process(error);

                            onFailure();
                        });
                }
                catch (Exception exception)
                {
                    ExceptionHandler?.Process(exception);

                    onFailure();
                }
            }
        }

        #endregion
    }
}
#endif
