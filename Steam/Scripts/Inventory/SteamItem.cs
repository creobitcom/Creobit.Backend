#if CREOBIT_BACKEND_STEAM && UNITY_STANDALONE
using Steamworks;
using System;

namespace Creobit.Backend.Inventory
{
    public sealed class SteamItem : ISteamItem
    {
        #region IItem

        IDefinition IItem.Definition => Definition;

        int? IItem.RemainingUses => InventoryItem.Quantity;

        void IItem.Consume(int count, Action onComplete, Action onFailure) => Consume(this, count, onComplete, onFailure);

        #endregion
        #region ISteamItem

        InventoryItem ISteamItem.InventoryItem => InventoryItem;

        #endregion
        #region SteamItem

        internal readonly IDefinition Definition;
        internal readonly InventoryItem InventoryItem;

        public SteamItem(IDefinition definition, InventoryItem inventoryItem)
        {
            Definition = definition;
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
