#if CREOBIT_BACKEND_PLAYFAB
using PlayFab.ClientModels;

namespace Creobit.Backend.Store
{
    internal sealed class PlayFabProduct : Product, IPlayFabProduct
    {
        #region IPlayFabProduct

        CatalogItem IPlayFabProduct.CatalogItem => CatalogItem;

        StoreItem IPlayFabProduct.StoreItem => StoreItem;

        #endregion
        #region PlayFabProduct

        private readonly CatalogItem CatalogItem;
        private readonly StoreItem StoreItem;

        public PlayFabProduct(string id, IPrice price, CatalogItem catalogItem, StoreItem storeItem) : base(id, price)
        {
            CatalogItem = catalogItem;
            StoreItem = storeItem;
        }

        #endregion
    }
}
#endif
