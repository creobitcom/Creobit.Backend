#if CREOBIT_BACKEND_APPSTOREPLAYFAB && CREOBIT_BACKEND_PLAYFAB
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using UnityEngine.Purchasing;
using UProduct = UnityEngine.Purchasing.Product;

namespace Creobit.Backend
{
    public sealed class AppStorePlayFabStore : IPlayFabStore
    {
        #region IStore

        IEnumerable<IProduct> IStore.Products => PlayFabStore.Products;

        void IStore.LoadProducts(Action onComplete, Action onFailure)
        {
            var purchasingModule = StandardPurchasingModule.Instance();
            var builder = ConfigurationBuilder.Instance(purchasingModule);
            var appleConfiguration = builder.Configure<IAppleConfiguration>();

            if (!appleConfiguration.canMakePayments)
            {
                var exception = new NotSupportedException($"\"{nameof(IAppleConfiguration)}.{nameof(IAppleConfiguration.canMakePayments)}\" is false!");

                ExceptionHandler?.Process(exception);

                onFailure();

                return;
            }

            foreach (var (ProductId, ProductType) in ProductMap)
            {
                builder.AddProduct(ProductId, ProductType);
            }

            _storeListener = new StoreListener();
            _storeListener.Initialized += OnInitialized;
            _storeListener.InitializeFailed += OnInitializeFailed;

            UnityPurchasing.Initialize(_storeListener, builder);

            void OnInitialized(object sender, InitializedEventArgs eventArgs)
            {
                _storeListener.Initialized -= OnInitialized;
                _storeListener.InitializeFailed -= OnInitializeFailed;

                _storeController = eventArgs.StoreController;

                PlayFabStore.LoadProducts(
                    () =>
                    {
                        ModifyProducts();

                        onComplete();
                    }, onFailure);
            }

            void OnInitializeFailed(object sender, InitializeFailedEventArgs eventArgs)
            {
                _storeListener.Initialized -= OnInitialized;
                _storeListener.InitializeFailed -= OnInitializeFailed;

                var exception = new Exception($"UnityPurchasing initialization failure! InitializationFailureReason: \"{eventArgs.InitializationFailureReason}\"");

                ExceptionHandler?.Process(exception);

                onFailure();
            }

            void ModifyProducts()
            {
                foreach (PlayFabProduct product in PlayFabStore.Products)
                {
                    product.Purchase = Purchase;

                    var itemId = PlayFabStore.GetItemId(((IProduct)product).Id);

                    if (string.IsNullOrWhiteSpace(itemId))
                    {
                        continue;
                    }

                    var uProduct = FindProduct(itemId);

                    if (uProduct == null)
                    {
                        continue;
                    }

                    var currencyId = PlayFabStore.GetCurrencyId("RM");
                    var metadata = uProduct.metadata;
                    var price = Convert.ToUInt32(metadata.localizedPrice * 100m);

                    product.SetPrice(currencyId, price, metadata.isoCurrencyCode);
                }
            }
        }

        #endregion
        #region IPlayFabStore

        string IPlayFabStore.CatalogVersion => PlayFabStore.CatalogVersion;

        string IPlayFabStore.StoreId => PlayFabStore.StoreId;

        IEnumerable<(string CurrencyId, string VirtualCurrency)> IPlayFabStore.CurrencyMap => PlayFabStore.CurrencyMap;

        IEnumerable<(string ProductId, string ItemId)> IPlayFabStore.ProductMap => PlayFabStore.ProductMap;

        #endregion
        #region AppStorePlayFabStore

        private readonly IPlayFabStore PlayFabStore;

        private IExtensionProvider _extensionProvider;
        private IStoreController _storeController;
        private StoreListener _storeListener;

        public AppStorePlayFabStore(IPlayFabStore playFabStore)
        {
            PlayFabStore = playFabStore;
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

        // ProductId - AppStore
        public IEnumerable<(string ProductId, ProductType ProductType)> ProductMap
        {
            get;
            set;
        } = Array.Empty<ValueTuple<string, ProductType>>();

        public string PublicKey
        {
            get;
            set;
        }

        private UProduct FindProduct(string productId)
        {
            if (_storeController == null)
            {
                return null;
            }

            var products = _storeController.products;
            var product = products.WithID(productId);

            return product;
        }

        private void InitiatePurchase(string productId, Action<UProduct> onComplete, Action onFailure)
        {
            var product = FindProduct(productId);

            _storeListener.ProcessPurchase += OnProcessPurchase;
            _storeListener.PurchaseFailed += OnPurchaseFailed;

            _storeController.InitiatePurchase(product);

            void OnProcessPurchase(object sender, ProcessPurchaseEventArgs eventArgs)
            {
                if (eventArgs.ProductId != productId)
                {
                    return;
                }

                _storeListener.ProcessPurchase -= OnProcessPurchase;
                _storeListener.PurchaseFailed -= OnPurchaseFailed;

                onComplete(eventArgs.PurchasedProduct);
            }

            void OnPurchaseFailed(object sender, PurchaseFailedEventArgs eventArgs)
            {
                if (eventArgs.ProductId != productId)
                {
                    return;
                }

                _storeListener.ProcessPurchase -= OnProcessPurchase;
                _storeListener.PurchaseFailed -= OnPurchaseFailed;

                var exception = new Exception($"Purchase failure! PurchaseFailureReason: \"{eventArgs.PurchaseFailureReason}\"");

                ExceptionHandler?.Process(exception);

                onFailure();
            }
        }

        private void Purchase(IProduct product, string currencyId, Action onComplete, Action onFailure)
        {
            var itemId = PlayFabStore.GetItemId(product.Id);
            var virtualCurrency = PlayFabStore.GetVirtualCurrency(currencyId);

            if (virtualCurrency == "RM")
            {
                InitiatePurchase(itemId,
                    purchasedProduct =>
                    {
                        ValidateIOSReceipt(purchasedProduct, onComplete, onFailure);
                    }, onFailure);
            }
            else
            {
                var price = product.GetPrice(currencyId) ?? 0;

                try
                {
                    PlayFabClientAPI.PurchaseItem(
                        new PurchaseItemRequest()
                        {
                            CatalogVersion = PlayFabStore.CatalogVersion,
                            ItemId = itemId,
                            Price = Convert.ToInt32(price),
                            StoreId = string.IsNullOrWhiteSpace(PlayFabStore.StoreId) ? null : PlayFabStore.StoreId,
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

        private void ValidateIOSReceipt(UProduct purchasedProduct, Action onComplete, Action onFailure)
        {
            var metadata = purchasedProduct.metadata;
            var appleExtensions = _extensionProvider.GetExtension<IAppleExtensions>();
            var receiptData = appleExtensions.GetTransactionReceiptForProduct(purchasedProduct);

            try
            {
                PlayFabClientAPI.ValidateIOSReceipt(
                    new ValidateIOSReceiptRequest()
                    {
                        CurrencyCode = metadata.isoCurrencyCode,
                        PurchasePrice = Convert.ToInt32(metadata.localizedPrice * 100m),
                        ReceiptData = receiptData
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

        private sealed class StoreListener : IStoreListener
        {
            #region IStoreListener

            void IStoreListener.OnInitialized(IStoreController controller, IExtensionProvider provider)
            {
                var eventArgs = new InitializedEventArgs(controller, provider);

                Initialized(this, eventArgs);
            }

            void IStoreListener.OnInitializeFailed(InitializationFailureReason reason)
            {
                var eventArgs = new InitializeFailedEventArgs(reason);

                InitializeFailed(this, eventArgs);
            }

            void IStoreListener.OnPurchaseFailed(UProduct product, PurchaseFailureReason reason)
            {
                var eventArgs = new PurchaseFailedEventArgs(product, reason);

                PurchaseFailed(this, eventArgs);
            }

            PurchaseProcessingResult IStoreListener.ProcessPurchase(PurchaseEventArgs args)
            {
                var eventArgs = new ProcessPurchaseEventArgs(args);

                ProcessPurchase(this, eventArgs);

                return PurchaseProcessingResult.Complete;
            }

            #endregion
            #region StoreListener

            public event EventHandler<InitializedEventArgs> Initialized = delegate { };

            public event EventHandler<InitializeFailedEventArgs> InitializeFailed = delegate { };

            public event EventHandler<PurchaseFailedEventArgs> PurchaseFailed = delegate { };

            public event EventHandler<ProcessPurchaseEventArgs> ProcessPurchase = delegate { };

            #endregion
        }

        private sealed class InitializedEventArgs : EventArgs
        {
            #region InitializedEventArgs

            public readonly IExtensionProvider ExtensionProvider;
            public readonly IStoreController StoreController;

            public InitializedEventArgs(IStoreController controller, IExtensionProvider provider)
            {
                ExtensionProvider = provider;
                StoreController = controller;
            }

            #endregion
        }

        private sealed class InitializeFailedEventArgs : EventArgs
        {
            #region InitializeFailedEventArgs

            public readonly InitializationFailureReason InitializationFailureReason;

            public InitializeFailedEventArgs(InitializationFailureReason reason)
            {
                InitializationFailureReason = reason;
            }

            #endregion
        }

        private sealed class ProcessPurchaseEventArgs : EventArgs
        {
            #region ProcessPurchaseEventArgs

            public readonly string ProductId;
            public readonly UProduct PurchasedProduct;

            public ProcessPurchaseEventArgs(PurchaseEventArgs eventArgs)
            {
                var purchasedProduct = eventArgs.purchasedProduct;
                var definition = purchasedProduct.definition;

                ProductId = definition.id;
                PurchasedProduct = purchasedProduct;
            }

            #endregion
        }

        private sealed class PurchaseFailedEventArgs : EventArgs
        {
            #region PurchaseFailedEventArgs

            public readonly string ProductId;
            public readonly PurchaseFailureReason PurchaseFailureReason;

            public PurchaseFailedEventArgs(UProduct product, PurchaseFailureReason reason)
            {
                var definition = product.definition;

                ProductId = definition.id;
                PurchaseFailureReason = reason;
            }

            #endregion
        }

        #endregion
    }
}
#endif
