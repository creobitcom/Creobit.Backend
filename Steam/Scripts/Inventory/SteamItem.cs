#if CREOBIT_BACKEND_STEAM && UNITY_STANDALONE
using Steamworks;
using System;

namespace Creobit.Backend.Inventory
{
    public sealed class SteamItem : ISteamItem
    {
        #region IItem

        IItemDefinition IItem.ItemDefinition => ItemDefinition;

        int? IItem.RemainingUses => InventoryItem.Quantity;

        void IItem.Consume(int count, Action onComplete, Action onFailure) => Consume(this, count, onComplete, onFailure);

        #endregion
        #region ISteamItem

        InventoryItem ISteamItem.InventoryItem => InventoryItem;

        #endregion
        #region SteamItem

        internal readonly IItemDefinition ItemDefinition;
        internal readonly InventoryItem InventoryItem;

        public SteamItem(IItemDefinition itemDefinition, InventoryItem inventoryItem)
        {
            ItemDefinition = itemDefinition;
            InventoryItem = inventoryItem;
        }

        internal Action<SteamItem, int, Action, Action> Consume
        {
            get;
            set;
        }

        #endregion
    }
}
#endif
