#if CREOBIT_BACKEND_PLAYFAB
using PlayFab.ClientModels;

namespace Creobit.Backend.Inventory
{
    internal sealed class PlayFabItem : Item, IPlayFabItem
    {
        #region IPlayFabItem

        CatalogItem IPlayFabItem.CatalogItem => CatalogItem;

        ItemInstance IPlayFabItem.ItemInstance => ItemInstance;

        #endregion
        #region PlayFabItem

        public PlayFabItem(string id) : base(id)
        {
        }

        public CatalogItem CatalogItem
        {
            get;
            set;
        }

        public ItemInstance ItemInstance
        {
            get;
            set;
        }

        #endregion
    }
}
#endif
