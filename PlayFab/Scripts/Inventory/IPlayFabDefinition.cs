#if CREOBIT_BACKEND_PLAYFAB
using PlayFab.ClientModels;

namespace Creobit.Backend.Inventory
{
    public interface IPlayFabDefinition : IDefinition
    {
        #region IPlayFabDefinition

        CatalogItem CatalogItem
        {
            get;
        }

        #endregion
    }
}
#endif
