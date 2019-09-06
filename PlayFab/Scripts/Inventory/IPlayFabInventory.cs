#if CREOBIT_BACKEND_PLAYFAB
using PlayFab.ClientModels;
using System.Collections.Generic;

namespace Creobit.Backend.Inventory
{
    public interface IPlayFabInventory : IInventory
    {
        #region IPlayFabInventory

        string CatalogVersion
        {
            get;
        }

        IEnumerable<(string DefinitionId, string PlayFabItemId)> DefinitionMap
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

        #endregion
    }
}
#endif
