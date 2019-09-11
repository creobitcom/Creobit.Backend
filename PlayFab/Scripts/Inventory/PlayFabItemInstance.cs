#if CREOBIT_BACKEND_PLAYFAB
using PlayFab.ClientModels;
using System;

namespace Creobit.Backend.Inventory
{
    internal sealed class PlayFabItemInstance : IPlayFabItemInstance
    {
        #region IItemInstance

        uint IItemInstance.Count => Convert.ToUInt32(ItemInstance.RemainingUses ?? 1);

        void IItemInstance.Consume(uint count, Action onComplete, Action onFailure) => Consume(this, count, onComplete, onFailure);

        #endregion
        #region IItemInstance<IPlayFabItemDefinition>

        IPlayFabItemDefinition IItemInstance<IPlayFabItemDefinition>.ItemDefinition => ItemDefinition;

        #endregion
        #region IPlayFabItemInstance

        ItemInstance IPlayFabItemInstance.NativeItemInstance => ItemInstance;

        #endregion
        #region PlayFabItemInstance

        public readonly IPlayFabItemDefinition ItemDefinition;
        public readonly ItemInstance ItemInstance;

        public PlayFabItemInstance(IPlayFabItemDefinition itemDefinition, ItemInstance itemInstance)
        {
            ItemDefinition = itemDefinition;
            ItemInstance = itemInstance;
        }

        public Action<PlayFabItemInstance, uint, Action, Action> Consume
        {
            get;
            set;
        }

        #endregion
    }
}
#endif
