#if CREOBIT_BACKEND_GOOGLEPLAY && CREOBIT_BACKEND_PLAYFAB && CREOBIT_BACKEND_UNITY && UNITY_ANDROID
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using UnityEngine.Purchasing;
using NativeProduct = UnityEngine.Purchasing.Product;

namespace Creobit.Backend.Store
{
    public sealed class GooglePlayPlayFabStore : IStore
    {
        #region IRefreshable

        void IRefreshable.Refresh(Action onComplete, Action onFailure)
        {
            PlayFabStore.Refresh(() => GooglePlayStore.Refresh(OnComplete, onFailure), onFailure);

            void OnComplete()
            {
                foreach (Product product in GooglePlayStore.Products)
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

                foreach (var product in GooglePlayStore.Products)
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

                foreach (var subscription in GooglePlayStore.Subscriptions)
                {
                    yield return subscription;
                }
            }
        }

        #endregion
        #region GooglePlayPlayFabStore

        public readonly IPlayFabStore PlayFabStore;
        public readonly IGooglePlayStore GooglePlayStore;

        private IExceptionHandler _exceptionHandler;
        private IPlayFabErrorHandler _playFabErrorHandler;

        public GooglePlayPlayFabStore(IPlayFabStore playFabStore, IGooglePlayStore googlePlayStore)
        {
            PlayFabStore = playFabStore;
            GooglePlayStore = googlePlayStore;
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
                    var receiptJson = nativeProduct.receipt;
                    var receipt = (Dictionary<string, object>)MiniJson.JsonDecode(receiptJson);
                    var payloadJson = (string)receipt["Payload"];
                    var payload = (Dictionary<string, object>)MiniJson.JsonDecode(payloadJson);
                    var json = (string)payload["json"];
                    var signature = (string)payload["signature"];

                    try
                    {
                        PlayFabClientAPI.ValidateGooglePlayPurchase(
                            new ValidateGooglePlayPurchaseRequest()
                            {
                                CatalogVersion = PlayFabStore.CatalogVersion,
                                CurrencyCode = metadata.isoCurrencyCode,
                                PurchasePrice = Convert.ToUInt32(metadata.localizedPrice * 100m),
                                ReceiptJson = json,
                                Signature = signature
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
