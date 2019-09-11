#if CREOBIT_BACKEND_STEAM && UNITY_STANDALONE
using Steamworks;

namespace Creobit.Backend.Inventory
{
    public static class SteamInventoryExtensions
    {
        #region SteamInventoryExtensions

        public static string FindItemDefinitionIdBySteamDefId(this ISteamInventory self, int steamDefId)
        {
            foreach (var (ItemDefinitionId, SteamDefId) in self.ItemDefinitionMap)
            {
                if (SteamDefId == steamDefId)
                {
                    return ItemDefinitionId;
                }
            }

            return null;
        }

        public static InventoryDef FindInventoryDefBySteamDefId(this ISteamInventory self, int steamDefId)
        {
            foreach (var inventoryDef in self.InventoryDefs)
            {
                if (inventoryDef.Id == steamDefId)
                {
                    return inventoryDef;
                }
            }

            return null;
        }

        #endregion
    }
}
#endif
