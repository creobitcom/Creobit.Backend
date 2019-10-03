#if CREOBIT_BACKEND_PLAYFAB && CREOBIT_BACKEND_STEAM && UNITY_STANDALONE
using PlayFab;
using PlayFab.ClientModels;
using Steamworks;
using System;
using System.Collections.Generic;
using UApplication = UnityEngine.Application;

namespace Creobit.Backend.Store
{
    public sealed class SteamPlayFabStore : PlayFabStore
    {
        #region PlayFabStore

        protected override void PurchaseViaRealCurrency(IProduct product, Action onComplete, Action onFailure)
        {
            if (UApplication.isEditor)
            {
                var exception = new NotSupportedException("Steam purchases is don't work in the Editor!");

                ExceptionHandler.Process(exception);

                onFailure();

                return;
            }

            if (!SteamUtils.IsOverlayEnabled)
            {
                var exception = new NotSupportedException("Steam overlay is disabled!");

                ExceptionHandler.Process(exception);

                onFailure();

                return;
            }

            var playFabProduct = (IPlayFabProduct)product;
            var catalogItem = playFabProduct.CatalogItem;

            StartPurchase();

            void StartPurchase()
            {
                try
                {
                    PlayFabClientAPI.StartPurchase(
                        new StartPurchaseRequest()
                        {
                            CatalogVersion = CatalogVersion,
                            Items = new List<ItemPurchaseRequest>()
                            {
                                new ItemPurchaseRequest()
                                {
                                    ItemId = catalogItem.ItemId,
                                    Quantity = 1
                                }
                            },
                            StoreId = StoreId
                        },
                        result =>
                        {
                            PayForPurchase(result);
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

            void PayForPurchase(StartPurchaseResult startPurchaseResult)
            {
                var paymentOptions = startPurchaseResult.PaymentOptions;
                var paymentOption = paymentOptions.Find(x => x.ProviderName == "Steam");
                var payForPurchaseResult = default(PayForPurchaseResult);

                WaitMicroTxnAuthorizationResponse(() => ConfirmPurchase(payForPurchaseResult), onFailure);

                try
                {
                    PlayFabClientAPI.PayForPurchase(
                        new PayForPurchaseRequest()
                        {
                            Currency = paymentOption.Currency,
                            OrderId = startPurchaseResult.OrderId,
                            ProviderName = paymentOption.ProviderName
                        },
                        result =>
                        {
                            payForPurchaseResult = result;
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

            void ConfirmPurchase(PayForPurchaseResult payForPurchaseResult)
            {
                try
                {
                    PlayFabClientAPI.ConfirmPurchase(
                        new ConfirmPurchaseRequest()
                        {
                            OrderId = payForPurchaseResult.OrderId
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

        #endregion
        #region SteamPlayFabStore

        public SteamPlayFabStore(string catalogVersion, string storeId) : base(catalogVersion, storeId)
        {
        }

        private void WaitMicroTxnAuthorizationResponse(Action onComplete, Action onFailure)
        {
            SteamUser.OnMicroTxnAuthorizationResponse += OnMicroTxnAuthorizationResponse;

            void OnMicroTxnAuthorizationResponse(AppId appid, ulong orderId, bool userAuthorized)
            {
                SteamUser.OnMicroTxnAuthorizationResponse -= OnMicroTxnAuthorizationResponse;

                if (userAuthorized)
                {
                    onComplete();
                }
                else
                {
                    onFailure();
                }
            }
        }

        #endregion
    }
}
#endif
