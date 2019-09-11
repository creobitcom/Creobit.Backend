#if CREOBIT_BACKEND_STEAM && UNITY_STANDALONE
using Steamworks;
using System.Collections.Generic;

namespace Creobit.Backend.Inventory
{
    public interface ISteamInventory : IInventory<ICurrencyDefinition, ICurrencyInstance<ICurrencyDefinition>, ISteamItemDefinition, ISteamItemInstance>
    {
        #region ISteamInventory

        InventoryDef[] InventoryDefs
        {
            get;
        }

        InventoryItem[] InventoryItems
        {
            get;
        }

        IEnumerable<(string ItemDefinitionId, int SteamDefId)> ItemDefinitionMap
        {
            get;
        }

        #endregion
    }
}
#endif
