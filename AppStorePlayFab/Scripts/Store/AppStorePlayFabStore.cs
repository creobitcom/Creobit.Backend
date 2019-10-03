#if CREOBIT_BACKEND_APPSTORE && CREOBIT_BACKEND_PLAYFAB && CREOBIT_BACKEND_UNITY && (UNITY_STANDALONE_OSX || UNITY_IOS)
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using NativeProduct = UnityEngine.Purchasing.Product;

namespace Creobit.Backend.Store
{
    public sealed class AppStorePlayFabStore : IStore
    {
        #region IRefreshable

        void IRefreshable.Refresh(Action onComplete, Action onFailure)
        {
            PlayFabStore.Refresh(() => AppStoreStore.Refresh(OnComplete, onFailure), onFailure);

            void OnComplete()
            {
                foreach (Product product in AppStoreStore.Products)
                {
                    OverridePurchaseDelegate(product);
                }

                onComplete();
            }
        }

        #endregion
        #region IStore

        IEnumerable<IProduct> IStore.Products
        {
            get
            {
                foreach (var product in PlayFabStore.Products)
                {
                    yield return product;
                }

                foreach (var product in AppStoreStore.Products)
                {
                    yield return product;
                }
            }
        }

        IEnumerable<ISubscription> IStore.Subscriptions
        {
            get
            {
                foreach (var subscription in PlayFabStore.Subscriptions)
                {
                    yield return subscription;
                }

                foreach (var subscription in AppStoreStore.Subscriptions)
                {
                    yield return subscription;
                }
            }
        }

        #endregion
        #region AppStorePlayFabStore

        public readonly IAppStoreStore AppStoreStore;
        public readonly IPlayFabStore PlayFabStore;

        private IExceptionHandler _exceptionHandler;
        private IPlayFabErrorHandler _playFabErrorHandler;

        public AppStorePlayFabStore(IPlayFabStore playFabStore, IAppStoreStore appStoreStore)
        {
            PlayFabStore = playFabStore;
            AppStoreStore = appStoreStore;
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

        private void OverridePurchaseDelegate(Product product)
        {
            var purchaseDelegate = product.PurchaseDelegate;

            if (product.PurchaseDelegate == Purchase)
            {
                return;
            }

            product.PurchaseDelegate = Purchase;

            void Purchase(IProduct unityProduct, Action onComplete, Action onFailure)
            {
                purchaseDelegate(unityProduct,
                    () =>
                    {
                        var nativeProduct = ((IUnityProduct)unityProduct).NativeProduct;

                        Validate(nativeProduct);
                    }, onFailure);

                void Validate(NativeProduct nativeProduct)
                {
                    var metadata = nativeProduct.metadata;
                    var appleExtensions = AppStoreStore.AppleExtensions;
                    var receiptData = appleExtensions.GetTransactionReceiptForProduct(nativeProduct);

                    try
                    {
                        PlayFabClientAPI.ValidateIOSReceipt(
                            new ValidateIOSReceiptRequest()
                            {
                                CatalogVersion = PlayFabStore.CatalogVersion,
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
        }

        #endregion
    }
}
#endif
