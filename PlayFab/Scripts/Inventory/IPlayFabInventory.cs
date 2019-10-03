#if CREOBIT_BACKEND_PLAYFAB
using Creobit.Backend.User;
using System.Collections.Generic;
using Creobit.Backend.Wallet;

namespace Creobit.Backend.Inventory
{
    public interface IPlayFabInventory : IInventory
    {
        #region IPlayFabInventory

        string CatalogVersion
        {
            get;
        }

        IEnumerable<(string ItemId, string PlayFabItemId)> ItemMap
        {
            get;
        }

        IPlayFabUser User
        {
            get;
        }

        IPlayFabWallet Wallet
        {
            get;
        }

        #endregion
    }
}
#endif
