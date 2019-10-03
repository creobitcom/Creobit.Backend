#if CREOBIT_BACKEND_PLAYFAB
using System.Collections.Generic;

namespace Creobit.Backend.Store
{
    public interface IPlayFabStore : IStore
    {
        #region IPlayFabStore

        string CatalogVersion
        {
            get;
        }

        IEnumerable<(string PriceId, string PlayFabVirtualCurrencyId)> PriceMap
        {
            get;
        }

        IEnumerable<(string ProductId, string PlayFabItemId)> ProductMap
        {
            get;
        }

        string StoreId
        {
            get;
        }

        #endregion
    }
}
#endif
