#if CREOBIT_BACKEND_PLAYFAB
using PlayFab.ClientModels;
using System;

namespace Creobit.Backend.Inventory
{
    public sealed class PlayFabItem : IPlayFabItem
    {
        #region IItem

        int? IItem.RemainingUses => ItemInstance.RemainingUses;

        void IItem.Consume(int count, Action onComplete, Action onFailure) => Consume(this, count, onComplete, onFailure);

        #endregion
        #region IItem<IPlayFabItemDefinition>

        IItemDefinition IItem<IPlayFabItemDefinition>.ItemDefinition => ItemDefinition;

        #endregion
        #region IPlayFabItem

        ItemInstance IPlayFabItem.ItemInstance => ItemInstance;

        #endregion
        #region PlayFabItem

        internal readonly IItemDefinition ItemDefinition;
        internal readonly ItemInstance ItemInstance;

        internal PlayFabItem(IItemDefinition itemDefinition, ItemInstance itemInstance)
        {
            ItemDefinition = itemDefinition;
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
