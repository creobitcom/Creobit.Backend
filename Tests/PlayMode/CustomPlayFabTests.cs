using System;
using System.Collections;
using Creobit.Backend.Auth;
using Creobit.Backend.Inventory;
using Creobit.Backend.Store;
using Creobit.Backend.User;
using Creobit.Backend.Wallet;
using UnityEngine.TestTools;

namespace Creobit.Backend.Tests.PlayMode
{
    public class CustomPlayFabTests : BasePlayFabTests
    {
        #region CustomPlayFabTests

#if CREOBIT_BACKEND_PLAYFAB

        private string _customId = "DFCD3F6447E4D116";

        [UnityTest]
        public IEnumerator Login_RefreshWalletInventoryStore()
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

            ICustomPlayFabAuth customPlayFabAuth = new CustomPlayFabAuth(playFabAuth, _customId);

            yield return PlayFabWhileHandler(customPlayFabAuth.Login);
            yield return PlayFabWhileHandler(playFabWallet.Refresh);
            yield return PlayFabWhileHandler(playFabInventory.Refresh);
            yield return PlayFabWhileHandler(playFabStore.Refresh);
        }
#endif

        #endregion
    }
}