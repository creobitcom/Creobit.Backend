#if CREOBIT_BACKEND_PLAYFAB
using PlayFab.ClientModels;

namespace Creobit.Backend.Inventory
{
    public interface IPlayFabItemDefinition : IItemDefinition
    {
        #region IPlayFabItemDefinition

        CatalogItem CatalogItem
        {
            get;
        }

        #endregion
    }
}
#endif
