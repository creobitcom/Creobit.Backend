#if CREOBIT_BACKEND_STEAM && UNITY_STANDALONE
using Steamworks;
using System;
using System.Collections.Generic;

namespace Creobit.Backend.Inventory
{
    internal sealed class SteamItemDefinition : ISteamItemDefinition
    {
        #region IItemDefinition

        IEnumerable<(ICurrencyDefinition CurrencyDefinition, uint Count)> IItemDefinition.BundledCurrencyDefinitions
            => Array.Empty<(ICurrencyDefinition CurrencyDefinition, uint Count)>();

        IEnumerable<(IItemDefinition ItemDefinition, uint Count)> IItemDefinition.BundledItemDefinitions
            => Array.Empty<(IItemDefinition ItemDefinition, uint Count)>();

        string IItemDefinition.Id => Id;

        #endregion
        #region IItemDefinition<TItemInstance>

        void IItemDefinition<ISteamItemInstance>.Grant(uint count, Action<IEnumerable<ISteamItemInstance>> onComplete, Action onFailure)
            => Grant(this, count, onComplete, onFailure);

        #endregion
        #region ISteamItemDefinition

        InventoryDef ISteamItemDefinition.InventoryDef => InventoryDef;

        #endregion
        #region SteamItemDefinition

        public readonly string Id;
        public readonly InventoryDef InventoryDef;

        public SteamItemDefinition(string id, InventoryDef inventoryDef)
        {
            Id = id;
            InventoryDef = inventoryDef;
        }

        public Action<SteamItemDefinition, uint, Action<IEnumerable<SteamItemInstance>>, Action> Grant
        {
            get;
            set;
        }

        #endregion
    }
}
#endif
