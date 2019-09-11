#if CREOBIT_BACKEND_PLAYFAB
using PlayFab.ClientModels;

namespace Creobit.Backend.Inventory
{
    public static class PlayFabInventoryExtensions
    {
        #region PlayFabInventoryExtensions

        public static CatalogItem FindCatalogItemByPlayFabItemId(this IPlayFabInventory self, string playFabItemId)
        {
            var getCatalogItemsResult = self.NativeGetCatalogItemsResult;

            if (getCatalogItemsResult == null)
            {
                return null;
            }

            foreach (var catalogItem in getCatalogItemsResult.Catalog)
            {
                if (catalogItem.ItemId == playFabItemId)
                {
                    return catalogItem;
                }
            }

            return null;
        }

        public static string FindCurrencyDefinitionIdByPlayFabVirtualCurrencyId(this IPlayFabInventory self, string playFabVirtualCurrencyId)
        {
            foreach (var (CurrencyDefinitionId, PlayFabVirtualCurrencyId) in self.CurrencyDefinitionMap)
            {
                if (PlayFabVirtualCurrencyId == playFabVirtualCurrencyId)
                {
                    return CurrencyDefinitionId;
                }
            }

            return null;
        }

        public static string FindItemDefinitionIdByPlayFabItemId(this IPlayFabInventory self, string playFabItemId)
        {
            foreach (var (ItemDefinitionId, PlayFabItemId) in self.ItemDefinitionMap)
            {
                if (PlayFabItemId == playFabItemId)
                {
                    return ItemDefinitionId;
                }
            }

            return null;
        }

        #endregion
    }
}
#endif
