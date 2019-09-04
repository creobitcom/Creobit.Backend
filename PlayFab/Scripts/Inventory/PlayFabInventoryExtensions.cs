#if CREOBIT_BACKEND_PLAYFAB
using PlayFab.ClientModels;

namespace Creobit.Backend.Inventory
{
    public static class PlayFabInventoryExtensions
    {
        #region PlayFabInventoryExtensions

        public static CatalogItem FindCatalogItemByPlayFabItemId(this IPlayFabInventory self, string playFabItemId)
        {
            var getCatalogItemsResult = self.GetCatalogItemsResult;

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

        public static string FindDefinitionIdByPlayFabItemId(this IPlayFabInventory self, string playFabItemId)
        {
            foreach (var (DefinitionId, PlayFabItemId) in self.DefinitionMap)
            {
                if (PlayFabItemId == playFabItemId)
                {
                    return DefinitionId;
                }
            }

            return null;
        }

        #endregion
    }
}
#endif
