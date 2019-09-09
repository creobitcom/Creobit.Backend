#if CREOBIT_BACKEND_PLAYFAB && CREOBIT_BACKEND_STEAM && UNITY_STANDALONE
using PlayFab;
using PlayFab.ClientModels;
using Steamworks;
using System;
using System.Collections.Generic;
using UApplication = UnityEngine.Application;

namespace Creobit.Backend.Store
{
    public sealed class SteamPlayFabStore : IPlayFabStore
    {
        #region IStore

        IEnumerable<IProduct> IStore.Products => PlayFabStore.Products;

        void IStore.LoadProducts(Action onComplete, Action onFailure)
        {
            PlayFabStore.LoadProducts(
                () =>
                {
                    ModifyProducts();

                    onComplete();
                }, onFailure);

            void ModifyProducts()
            {
                foreach (PlayFabProduct product in PlayFabStore.Products)
                {
                    product.Purchase = Purchase;
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
        #region SteamPlayFabStore

        private readonly IPlayFabStore PlayFabStore;

        private IExceptionHandler _exceptionHandler;
        private IPlayFabErrorHandler _playFabErrorHandler;

        public SteamPlayFabStore(IPlayFabStore playFabStore)
        {
            PlayFabStore = playFabStore;
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

        private void Purchase(IProduct product, string currencyId, Action onComplete, Action onFailure)
        {
            var itemId = this.GetItemId(product.Id);
            var virtualCurrency = this.GetVirtualCurrency(currencyId);

            if (virtualCurrency == "RM")
            {
                if (UApplication.isEditor)
                {
                    var exception = new NotSupportedException("Steam purchases is don't work in the Editor!");

                    ExceptionHandler?.Process(exception);

                    onFailure();

                    return;
                }

                StartPurchase();
            }
            else
            {
                PurchaseItem();
            }

            void StartPurchase()
            {
                try
                {
                    PlayFabClientAPI.StartPurchase(
                        new StartPurchaseRequest()
                        {
                            CatalogVersion = PlayFabStore.CatalogVersion,
                            Items = new List<ItemPurchaseRequest>()
                            {
                                new ItemPurchaseRequest()
                                {
                                    ItemId = itemId,
                                    Quantity = 1
                                }
                            },
                            StoreId = PlayFabStore.StoreId
                        },
                        result =>
                        {
                            PayForPurchase(result);
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

            void PurchaseItem()
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
                            StoreId = PlayFabStore.StoreId,
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

        private void WaitMicroTxnAuthorizationResponse(Action onComplete, Action onFailure)
        {
            if (!SteamUtils.IsOverlayEnabled)
            {
                var exception = new NotSupportedException("Steam overlay is disabled!");

                ExceptionHandler?.Process(exception);

                onFailure();

                return;
            }

            Steamworks.SteamUser.OnMicroTxnAuthorizationResponse += OnMicroTxnAuthorizationResponse;

            void OnMicroTxnAuthorizationResponse(AppId appid, ulong orderId, bool userAuthorized)
            {
                Steamworks.SteamUser.OnMicroTxnAuthorizationResponse -= OnMicroTxnAuthorizationResponse;

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
