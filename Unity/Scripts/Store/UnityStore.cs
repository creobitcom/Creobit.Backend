#if CREOBIT_BACKEND_UNITY
using System;
using System.Collections.Generic;
using UnityEngine.Purchasing;
using NativeProduct = UnityEngine.Purchasing.Product;

namespace Creobit.Backend.Store
{
    public abstract class UnityStore : IUnityStore
    {
        #region IRefreshable

        void IRefreshable.Refresh(Action onComplete, Action onFailure)
        {
            var configurationBuilder = CreateConfigurationBuilder();

            _storeListener = new StoreListener();
            _storeListener.Initialized += OnInitialized;
            _storeListener.InitializeFailed += OnInitializeFailed;

            UnityPurchasing.Initialize(_storeListener, configurationBuilder);

            void OnInitialized(object sender, InitializedEventArgs eventArgs)
            {
                _storeListener.Initialized -= OnInitialized;
                _storeListener.InitializeFailed -= OnInitializeFailed;

                _storeListener.ProcessPurchase += OnPurchaseProcessed; 

                ExtensionProvider = eventArgs.ExtensionProvider;
                StoreController = eventArgs.StoreController;

                UpdateProducts();
                UpdateSubscriptions();

                onComplete();
            }

            void OnInitializeFailed(object sender, InitializeFailedEventArgs eventArgs)
            {
                _storeListener.Initialized -= OnInitialized;
                _storeListener.InitializeFailed -= OnInitializeFailed;

                var exception = new Exception($"Initialize failed! {nameof(eventArgs.InitializationFailureReason)}: \"{eventArgs.InitializationFailureReason}\"");

                ExceptionHandler.Process(exception);

                onFailure();
            }

            void OnPurchaseProcessed(object sender, ProcessPurchaseEventArgs eventArgs)
            {
                PurchaseProcessed?.Invoke(sender, eventArgs);
            }
        }

        #endregion
        #region IStore

        IEnumerable<IProduct> IStore.Products => Products;

        IEnumerable<ISubscription> IStore.Subscriptions => Subscriptions;

        #endregion
        #region IUnityStore

        IEnumerable<(string ProductId, (string Id, bool Consumable) NativeProduct)> IUnityStore.ProductMap => ProductMap;

        IEnumerable<(string SubscriptionId, string NativeProductId)> IUnityStore.SubscriptionMap => SubscriptionMap;

        public event EventHandler PurchaseProcessed;

        #endregion
        #region UnityStore

        private IList<IProduct> _products;
        private StoreListener _storeListener;
        private IList<ISubscription> _subscription;

        private IEnumerable<(string ProductId, (string Id, bool Consumable) NativeProduct)> _productMap;
        private IEnumerable<(string SubscriptionId, string NativeProductId)> _subscriptionMap;

        private IExceptionHandler _exceptionHandler;

        public IExceptionHandler ExceptionHandler
        {
            get => _exceptionHandler ?? Backend.ExceptionHandler.Default;
            set => _exceptionHandler = value;
        }

        protected IExtensionProvider ExtensionProvider
        {
            get;
            private set;
        }

        public IEnumerable<(string ProductId, (string Id, bool Consumable) NativeProduct)> ProductMap
        {
            get => _productMap ?? Array.Empty<(string ProductId, (string Id, bool Consumable) NativeProduct)>();
            set => _productMap = value;
        }

        private IList<IProduct> Products
        {
            get => _products ?? Array.Empty<IProduct>();
            set => _products = value;
        }

        protected IStoreController StoreController
        {
            get;
            private set;
        }

        public IEnumerable<(string SubscriptionId, string NativeProductId)> SubscriptionMap
        {
            get => _subscriptionMap ?? Array.Empty<(string SubscriptionId, string NativeProductId)>();
            set => _subscriptionMap = value;
        }

        private IList<ISubscription> Subscriptions
        {
            get => _subscription ?? Array.Empty<ISubscription>();
            set => _subscription = value;
        }

        protected virtual ConfigurationBuilder CreateConfigurationBuilder()
        {
            var purchasingModule = StandardPurchasingModule.Instance();
            var configurationBuilder = ConfigurationBuilder.Instance(purchasingModule);

            foreach (var (ProductId, NativeProduct) in ProductMap)
            {
                configurationBuilder.AddProduct(NativeProduct.Id, NativeProduct.Consumable ? ProductType.Consumable : ProductType.NonConsumable);
            }

            foreach (var (ProductId, NativeProductId) in SubscriptionMap)
            {
                configurationBuilder.AddProduct(NativeProductId, ProductType.Subscription);
            }

            return configurationBuilder;
        }

        private DateTime? GetExpireDate(ISubscription subscription)
        {
            var unitySubscription = (IUnitySubscription)subscription;
            var nativeProduct = unitySubscription.NativeProduct;

            if (!nativeProduct.hasReceipt)
            {
                return null;
            }

            var subscriptionManager = new SubscriptionManager(nativeProduct, null);
            var subscriptionInfo = subscriptionManager.getSubscriptionInfo();
            var expireDate = subscriptionInfo.getExpireDate();

            return expireDate;
        }

        private bool IsCanceled(ISubscription subscription)
        {
            var unitySubscription = (IUnitySubscription)subscription;
            var nativeProduct = unitySubscription.NativeProduct;

            if (!nativeProduct.hasReceipt)
            {
                return false;
            }

            var subscriptionManager = new SubscriptionManager(nativeProduct, null);
            var subscriptionInfo = subscriptionManager.getSubscriptionInfo();
            var cancelled = subscriptionInfo.isCancelled();

            return cancelled == Result.True;
        }

        private bool IsExpired(ISubscription subscription)
        {
            var unitySubscription = (IUnitySubscription)subscription;
            var nativeProduct = unitySubscription.NativeProduct;

            if (!nativeProduct.hasReceipt)
            {
                return false;
            }

            var subscriptionManager = new SubscriptionManager(nativeProduct, null);
            var subscriptionInfo = subscriptionManager.getSubscriptionInfo();
            var expired = subscriptionInfo.isExpired();

            return expired == Result.True;
        }

        private bool IsSubscribed(ISubscription subscription)
        {
            var unitySubscription = (IUnitySubscription)subscription;
            var nativeProduct = unitySubscription.NativeProduct;

            if (!nativeProduct.hasReceipt)
            {
                return false;
            }

            var subscriptionManager = new SubscriptionManager(nativeProduct, null);
            var subscriptionInfo = subscriptionManager.getSubscriptionInfo();
            var subscribed = subscriptionInfo.isSubscribed();

            return subscribed == Result.True;
        }

        private void Purchase(IPurchasableItem purchasableItem, Action onComplete, Action onFailure)
        {
            var unityProduct = (IUnityProduct)purchasableItem;

            _storeListener.ProcessPurchase += OnProcessPurchase;
            _storeListener.PurchaseFailed += OnPurchaseFailed;

            StoreController.InitiatePurchase(unityProduct.NativeProduct);

            void OnProcessPurchase(object sender, ProcessPurchaseEventArgs eventArgs)
            {
                _storeListener.ProcessPurchase -= OnProcessPurchase;
                _storeListener.PurchaseFailed -= OnPurchaseFailed;

                ((UnityProduct)purchasableItem).NativeProduct = eventArgs.NativeProduct;

                onComplete();
            }

            void OnPurchaseFailed(object sender, PurchaseFailedEventArgs eventArgs)
            {
                _storeListener.ProcessPurchase -= OnProcessPurchase;
                _storeListener.PurchaseFailed -= OnPurchaseFailed;

                var exception = new Exception($"Purchase failed! {nameof(eventArgs.PurchaseFailureReason)}: \"{eventArgs.PurchaseFailureReason}\"");

                ExceptionHandler.Process(exception);

                onFailure();
            }
        }

        private void Purchase(ISubscription subscription, Action onComplete, Action onFailure)
        {
            var unitySubscription = (IUnitySubscription)subscription;

            _storeListener.ProcessPurchase += OnProcessPurchase;
            _storeListener.PurchaseFailed += OnPurchaseFailed;

            StoreController.InitiatePurchase(unitySubscription.NativeProduct);

            void OnProcessPurchase(object sender, ProcessPurchaseEventArgs eventArgs)
            {
                _storeListener.ProcessPurchase -= OnProcessPurchase;
                _storeListener.PurchaseFailed -= OnPurchaseFailed;

                var subscriptionManager = new SubscriptionManager(eventArgs.NativeProduct, null);
                var subscriptionInfo = subscriptionManager.getSubscriptionInfo();

                ((UnitySubscription)subscription).NativeProduct = eventArgs.NativeProduct;

                onComplete();
            }

            void OnPurchaseFailed(object sender, PurchaseFailedEventArgs eventArgs)
            {
                _storeListener.ProcessPurchase -= OnProcessPurchase;
                _storeListener.PurchaseFailed -= OnPurchaseFailed;

                var exception = new Exception($"Purchase failed! {nameof(eventArgs.PurchaseFailureReason)}: \"{eventArgs.PurchaseFailureReason}\"");

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
                var nativeProducts = StoreController.products;

                foreach (var (ProductId, NativeProduct) in ProductMap)
                {
                    var nativeProduct = nativeProducts.WithID(NativeProduct.Id);

                    if (nativeProduct == null)
                    {
                        var exception = new Exception($"The NativeProduct is not found for the NativeProductId \"{NativeProduct.Id}\"!");

                        ExceptionHandler.Process(exception);

                        continue;
                    }

                    var metadata = nativeProduct.metadata;
                    var value = Convert.ToUInt32(metadata.localizedPrice * 100m);
                    var price = new Price(null, metadata.isoCurrencyCode, value);
                    var product = new UnityProduct(ProductId, price)
                    {
                        NativeProduct = nativeProduct,
                        PurchaseDelegate = Purchase
                    };

                    products.Add(product);
                }

                return products;
            }
        }

        private void UpdateSubscriptions()
        {
            Subscriptions = CreateSubscriptions();

            List<ISubscription> CreateSubscriptions()
            {
                var subscription = new List<ISubscription>();
                var nativeProducts = StoreController.products;

                foreach (var (SubscriptionId, NativeProductId) in SubscriptionMap)
                {
                    var nativeProduct = nativeProducts.WithID(NativeProductId);

                    if (nativeProduct == null)
                    {
                        var exception = new Exception($"The NativeProduct is not found for the NativeProductId \"{NativeProductId}\"!");

                        ExceptionHandler.Process(exception);

                        continue;
                    }

                    var metadata = nativeProduct.metadata;
                    var value = Convert.ToUInt32(metadata.localizedPrice * 100m);
                    var price = new Price(null, metadata.isoCurrencyCode, value);
                    var product = new UnitySubscription(SubscriptionId, price)
                    {
                        NativeProduct = nativeProduct,
                        GetExpireDateDelegate = GetExpireDate,
                        IsCanceledDelegate = IsCanceled,
                        IsExpiredDelegate = IsExpired,
                        IsSubscribedDelegate = IsSubscribed,
                        PurchaseDelegate = Purchase
                    };

                    subscription.Add(product);
                }

                return subscription;
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

            void IStoreListener.OnPurchaseFailed(NativeProduct product, PurchaseFailureReason reason)
            {
                var eventArgs = new PurchaseFailedEventArgs(product, reason);

                PurchaseFailed(this, eventArgs);
            }

            PurchaseProcessingResult IStoreListener.ProcessPurchase(PurchaseEventArgs purchaseEventArgs)
            {
                var eventArgs = new ProcessPurchaseEventArgs(purchaseEventArgs.purchasedProduct);

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

            public readonly NativeProduct NativeProduct;

            public ProcessPurchaseEventArgs(NativeProduct nativeProduct)
            {
                NativeProduct = nativeProduct;
            }

            #endregion
        }

        private sealed class PurchaseFailedEventArgs : EventArgs
        {
            #region PurchaseFailedEventArgs

            public readonly NativeProduct NativeProduct;
            public readonly PurchaseFailureReason PurchaseFailureReason;

            public PurchaseFailedEventArgs(NativeProduct product, PurchaseFailureReason reason)
            {
                NativeProduct = product;
                PurchaseFailureReason = reason;
            }

            #endregion
        }

        #endregion
    }
}
#endif
