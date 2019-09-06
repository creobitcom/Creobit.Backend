#if CREOBIT_BACKEND_STEAM && UNITY_STANDALONE
using Steamworks;
using System.Collections.Generic;

namespace Creobit.Backend.Inventory
{
    public interface ISteamInventory : IInventory
    {
        #region ISteamInventory

        IEnumerable<(string DefinitionId, int SteamDefId)> DefinitionMap
        {
            get;
        }

        InventoryDef[] InventoryDefs
        {
            get;
        }

        InventoryItem[] InventoryItems
        {
            get;
        }

        #endregion
    }
}
#endif
