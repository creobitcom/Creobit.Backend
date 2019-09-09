#if CREOBIT_BACKEND_STEAM && UNITY_STANDALONE
using Steamworks;
using System;
using System.Collections.Generic;

namespace Creobit.Backend.Inventory
{
    public sealed class SteamItemDefinition : ISteamItemDefinition
    {
        #region IItemDefinition

        IEnumerable<(IItemDefinition ItemDefinition, uint Count)> IItemDefinition.BundledItemDefinitions => Array.Empty<(IItemDefinition ItemDefinition, uint Count)>();

        string IItemDefinition.Id => Id;

        #endregion
        #region ISteamItemDefinition

        InventoryDef ISteamItemDefinition.InventoryDef => InventoryDef;

        #endregion
        #region SteamItemDefinition

        internal readonly string Id;
        internal readonly InventoryDef InventoryDef;

        internal SteamItemDefinition(string id, InventoryDef inventoryDef)
        {
            Id = id;
            InventoryDef = inventoryDef;
        }

        #endregion
    }
}
#endif
