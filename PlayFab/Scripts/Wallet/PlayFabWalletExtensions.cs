#if CREOBIT_BACKEND_PLAYFAB
namespace Creobit.Backend.Wallet
{
    public static class PlayFabWalletExtensions
    {
        #region PlayFabWalletExtensions

        public static string FindCurrencyId(this IPlayFabWallet self, string playFabVirtualCurrencyId)
        {
            foreach (var (CurrencyId, PlayFabVirtualCurrencyId) in self.CurrencyMap)
            {
                if (PlayFabVirtualCurrencyId == playFabVirtualCurrencyId)
                {
                    return CurrencyId;
                }
            }

            return null;
        }

        public static string FindPlayFabVirtualCurrencyId(this IPlayFabWallet self, string currencyId)
        {
            foreach (var (CurrencyId, PlayFabVirtualCurrencyId) in self.CurrencyMap)
            {
                if (CurrencyId == currencyId)
                {
                    return PlayFabVirtualCurrencyId;
                }
            }

            return null;
        }

        #endregion
    }
}
#endif
