#if CREOBIT_BACKEND_STEAM && UNITY_STANDALONE
using Steamworks;
using System;
using System.Collections.Generic;

namespace Creobit.Backend.Inventory
{
    public sealed class SteamDefinition : ISteamDefinition
    {
        #region IDefinition

        IEnumerable<(IDefinition Definition, uint Count)> IDefinition.BundledDefinitions => Array.Empty<(IDefinition Definition, uint Count)>();

        string IDefinition.Id => Id;

        #endregion
        #region ISteamDefinition

        InventoryDef ISteamDefinition.InventoryDef => InventoryDef;

        #endregion
        #region SteamDefinition

        internal readonly string Id;
        internal readonly InventoryDef InventoryDef;

        internal SteamDefinition(string id, InventoryDef inventoryDef)
        {
            Id = id;
            InventoryDef = inventoryDef;
        }

        #endregion
    }
}
#endif
