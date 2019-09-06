#if CREOBIT_BACKEND_PLAYFAB
using PlayFab.ClientModels;
using System;

namespace Creobit.Backend.Inventory
{
    public sealed class PlayFabItem : IPlayFabItem
    {
        #region IItem

        IDefinition IItem.Definition => Definition;

        int? IItem.RemainingUses => ItemInstance.RemainingUses;

        void IItem.Consume(int count, Action onComplete, Action onFailure) => Consume(this, count, onComplete, onFailure);

        #endregion
        #region IPlayFabItem

        ItemInstance IPlayFabItem.ItemInstance => ItemInstance;

        #endregion
        #region PlayFabItem

        internal readonly IDefinition Definition;
        internal readonly ItemInstance ItemInstance;

        internal PlayFabItem(IDefinition definition, ItemInstance itemInstance)
        {
            Definition = definition;
            ItemInstance = itemInstance;
        }

        internal Action<PlayFabItem, int, Action, Action> Consume
        {
            get;
            set;
        }

        #endregion
    }
}
#endif
