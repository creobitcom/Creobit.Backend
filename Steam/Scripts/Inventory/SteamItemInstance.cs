#if CREOBIT_BACKEND_STEAM && UNITY_STANDALONE
using Steamworks;
using System;

namespace Creobit.Backend.Inventory
{
    internal sealed class SteamItemInstance : ISteamItemInstance
    {
        #region IItemInstance

        uint IItemInstance.Count => Math.Max(Convert.ToUInt32(InventoryItem.Quantity), 1);

        void IItemInstance.Consume(uint count, Action onComplete, Action onFailure) => Consume(this, count, onComplete, onFailure);

        #endregion
        #region IItemInstance<ISteamItemDefinition>

        ISteamItemDefinition IItemInstance<ISteamItemDefinition>.ItemDefinition => ItemDefinition;

        #endregion
        #region ISteamItemInstance

        InventoryItem ISteamItemInstance.InventoryItem => InventoryItem;

        #endregion
        #region SteamItemInstance

        public readonly ISteamItemDefinition ItemDefinition;
        public readonly InventoryItem InventoryItem;

        public SteamItemInstance(ISteamItemDefinition itemDefinition, InventoryItem inventoryItem)
        {
            ItemDefinition = itemDefinition;
            InventoryItem = inventoryItem;
        }

        public Action<SteamItemInstance, uint, Action, Action> Consume
        {
            get;
            set;
        }

        #endregion
    }
}
#endif
