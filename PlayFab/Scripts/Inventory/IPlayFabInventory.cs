#if CREOBIT_BACKEND_PLAYFAB
using PlayFab.ClientModels;
using System.Collections.Generic;

namespace Creobit.Backend.Inventory
{
    public interface IPlayFabInventory : IInventory<IPlayFabCurrencyDefinition, IPlayFabCurrencyInstance, IPlayFabItemDefinition, IPlayFabItemInstance>
    {
        #region IPlayFabInventory

        string CatalogVersion
        {
            get;
        }

        IEnumerable<(string CurrencyDefinitionId, string PlayFabVirtualCurrencyId)> CurrencyDefinitionMap
        {
            get;
        }

        IEnumerable<(string ItemDefinitionId, string PlayFabItemId)> ItemDefinitionMap
        {
            get;
        }

        GetCatalogItemsResult NativeGetCatalogItemsResult
        {
            get;
        }

        GetPlayerCombinedInfoResultPayload NativeGetPlayerCombinedInfoResultPayload
        {
            get;
        }

        #endregion
    }
}
#endif
