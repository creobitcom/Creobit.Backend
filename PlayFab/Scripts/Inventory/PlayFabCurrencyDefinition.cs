#if CREOBIT_BACKEND_PLAYFAB
using PlayFab.ClientModels;
using System;

namespace Creobit.Backend.Inventory
{
    internal sealed class PlayFabCurrencyDefinition : IPlayFabCurrencyDefinition
    {
        #region ICurrencyDefinition

        string ICurrencyDefinition.Id => Id;

        #endregion
        #region IItemDefinition<TItemInstance>

        void ICurrencyDefinition<IPlayFabCurrencyInstance>.Grant(uint count, Action<IPlayFabCurrencyInstance> onComplete, Action onFailure)
            => Grant(this, count, onComplete, onFailure);

        #endregion
        #region IPlayFabCurrencyDefinition

        VirtualCurrencyRechargeTime IPlayFabCurrencyDefinition.NativeVirtualCurrencyRechargeTime => VirtualCurrencyRechargeTime;

        #endregion
        #region PlayFabCurrencyDefinition

        public readonly string Id;
        public readonly VirtualCurrencyRechargeTime VirtualCurrencyRechargeTime;

        public PlayFabCurrencyDefinition(string id, VirtualCurrencyRechargeTime virtualCurrencyRechargeTime)
        {
            Id = id;
            VirtualCurrencyRechargeTime = virtualCurrencyRechargeTime;
        }

        public Action<PlayFabCurrencyDefinition, uint, Action<PlayFabCurrencyInstance>, Action> Grant
        {
            get;
            set;
        }

        #endregion
    }
}
#endif
