#if CREOBIT_BACKEND_PLAYFAB
using System.Collections.Generic;

namespace Creobit.Backend.Wallet
{
    public interface IPlayFabWallet : IWallet
    {
        #region IPlayFabWallet

        IEnumerable<(string CurrencyId, string PlayFabVirtualCurrencyId)> CurrencyMap
        {
            get;
        }

        #endregion
    }
}
#endif
