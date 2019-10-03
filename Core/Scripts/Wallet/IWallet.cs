using System.Collections.Generic;

namespace Creobit.Backend.Wallet
{
    public interface IWallet : IRefreshable
    {
        #region IWallet

        IEnumerable<ICurrency> Currencies
        {
            get;
        }

        #endregion
    }
}
