#if CREOBIT_BACKEND_PLAYFAB
namespace Creobit.Backend.Inventory
{
    public static class PlayFabInventoryExtensions
    {
        #region PlayFabInventoryExtensions

        public static string FindItemId(this IPlayFabInventory self, string playFabItemId)
        {
            foreach (var (ItemId, PlayFabItemId) in self.ItemMap)
            {
                if (PlayFabItemId == playFabItemId)
                {
                    return ItemId;
                }
            }

            return null;
        }

        public static string FindPlayFabItemId(this IPlayFabInventory self, string itemId)
        {
            foreach (var (ItemId, PlayFabItemId) in self.ItemMap)
            {
                if (ItemId == itemId)
                {
                    return PlayFabItemId;
                }
            }

            return null;
        }

        #endregion
    }
}
#endif
