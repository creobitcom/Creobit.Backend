#if CREOBIT_BACKEND_PLAYFAB
using PlayFab.ClientModels;
using System.Collections.Generic;

namespace Creobit.Backend.Inventory
{
    public interface IPlayFabInventory : IInventory<IPlayFabItemDefinition, IPlayFabItem>
    {
        #region IPlayFabInventory

        string CatalogVersion
        {
            get;
        }

        GetCatalogItemsResult GetCatalogItemsResult
        {
            get;
        }

        GetUserInventoryResult GetUserInventoryResult
        {
            get;
        }

        IEnumerable<(string ItemDefinitionId, string PlayFabItemId)> ItemDefinitionMap
        {
            get;
        }

        #endregion
    }
}
#endif
