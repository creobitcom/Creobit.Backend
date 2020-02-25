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
    public class SteamPlayFabTests : BasePlayFabTests
    {
        #region SteamPlayFabTests

#if CREOBIT_BACKEND_PLAYFAB && CREOBIT_BACKEND_STEAM && UNITY_STANDALONE

        private uint _appId = 695720;

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

            var steamAuth = new SteamAuth(_appId);

            ISteamPlayFabAuth steamPlayFabAuth = new SteamPlayFabAuth(playFabAuth, steamAuth);
            IPlayFabStore steamPlayFabStore = new SteamPlayFabStore(String.Empty, String.Empty)
            {
                PriceMap = PlayFabStorePriceMap,
                ProductMap = PlayFabStoreProductMap
            };
            var steamUser = new SteamUser();
            IUser steamPlayFabUser = new SteamPlayFabUser(playFabUser, steamUser);

            yield return PlayFabWhileHandler(steamPlayFabAuth.Login);
            yield return PlayFabWhileHandler(playFabWallet.Refresh);
            yield return PlayFabWhileHandler(playFabInventory.Refresh);
            yield return PlayFabWhileHandler(steamPlayFabStore.Refresh);
            yield return PlayFabWhileHandler(steamPlayFabUser.Refresh);
        }
#endif

        #endregion
    }
}