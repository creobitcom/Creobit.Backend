#if CREOBIT_BACKEND_PLAYFAB
using PlayFab.ClientModels;

namespace Creobit.Backend.Store
{
    public interface IPlayFabProduct : IProduct
    {
        #region IPlayFabProduct

        CatalogItem CatalogItem
        {
            get;
        }

        StoreItem StoreItem
        {
            get;
        }

        #endregion
    }
}
#endif
