#if CREOBIT_BACKEND_PLAYFAB
using PlayFab.ClientModels;

namespace Creobit.Backend.Inventory
{
    public interface IPlayFabItem : IItem
    {
        #region IPlayFabItem

        CatalogItem CatalogItem
        {
            get;
        }

        ItemInstance ItemInstance
        {
            get;
        }

        #endregion
    }
}
#endif
