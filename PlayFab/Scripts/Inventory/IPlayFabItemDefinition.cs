#if CREOBIT_BACKEND_PLAYFAB
using PlayFab.ClientModels;

namespace Creobit.Backend.Inventory
{
    public interface IPlayFabItemDefinition : IItemDefinition<IPlayFabItemInstance>
    {
        #region IPlayFabItemDefinition

        CatalogItem NativeCatalogItem
        {
            get;
        }

        #endregion
    }
}
#endif
