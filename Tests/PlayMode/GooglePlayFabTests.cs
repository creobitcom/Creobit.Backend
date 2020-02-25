using System;
using System.Collections;
using System.Collections.Generic;
using Creobit.Backend.Auth;
using Creobit.Backend.Inventory;
using Creobit.Backend.Store;
using Creobit.Backend.User;
using Creobit.Backend.Wallet;

namespace Creobit.Backend.Tests.PlayMode
{
    public class GooglePlayFabTests : BasePlayFabTests
    {
        #region GooglePlayFabTests

#if CREOBIT_BACKEND_GOOGLEPLAY && CREOBIT_BACKEND_PLAYFAB && CREOBIT_BACKEND_UNITY && UNITY_ANDROID
        public override IEnumerator Login_RefreshWalletInventoryStore()
        {
            var playFabAuth = new PlayFabAuth(_titleId);
            var playFabUser = new PlayFabUser(playFabAuth);
            IPlayFabWallet playFabWallet = new PlayFabWallet
            {
                CurrencyMap = PlayFabWalletCurrencyMap
            };
            IPlayFabInventory playFabInventory = new PlayFabInventory(String.Empty, playFabUser, playFabWallet)
            {
                ItemMap = PlayFabInventoryItemMap
            };
            IPlayFabStore playFabStore = new PlayFabStore(String.Empty, String.Empty)
            {
                PriceMap = PlayFabStorePriceMap,
                ProductMap = PlayFabStoreProductMap
            };

            IGooglePlayAuth googlePlayAuth = new GooglePlayAuth();
            IPlayFabStore googlePlayStore = new GooglePlayStore(_publicKey)
            {
                ProductMap = new List<(string ProductId, (string Id, bool Consumable) NativeProduct)>(),
                SubscriptionMap = new List<(string SubscriptionId, string NativeProductId)>()
            };

            IGooglePlayPlayFabAuth googlePlayPlayFabAuth = new GooglePlayPlayFabAuth(playFabAuth, googlePlayAuth);
            IStore googlePlayPlayFabStore = new GooglePlayPlayFabStore(playFabStore, googlePlayStore);

            yield return PlayFabWhileHandler(googlePlayPlayFabAuth.Login);
            yield return PlayFabWhileHandler(playFabWallet.Refresh);
            yield return PlayFabWhileHandler(playFabInventory.Refresh);
            yield return PlayFabWhileHandler(googlePlayPlayFabStore.Refresh);
        }
#endif

        #endregion
    }
}