#if CREOBIT_BACKEND_PLAYFAB
using System;

namespace Creobit.Backend.Inventory
{
    internal sealed class PlayFabCurrencyInstance : IPlayFabCurrencyInstance
    {
        #region ICurrencyInstance

        uint ICurrencyInstance.Count => Count;

        void ICurrencyInstance.Consume(uint count, Action onComplete, Action onFailure) => Consume(this, count, onComplete, onFailure);

        #endregion
        #region ICurrencyInstance<IPlayFabCurrencyDefinition>

        IPlayFabCurrencyDefinition ICurrencyInstance<IPlayFabCurrencyDefinition>.CurrencyDefinition => CurrencyDefinition;

        #endregion
        #region PlayFabCurrencyInstance

        public readonly IPlayFabCurrencyDefinition CurrencyDefinition;
        public readonly uint Count;

        public PlayFabCurrencyInstance(IPlayFabCurrencyDefinition currencyDefinition, uint count)
        {
            CurrencyDefinition = currencyDefinition;
            Count = count;
        }

        public Action<PlayFabCurrencyInstance, uint, Action, Action> Consume
        {
            get;
            set;
        }

        #endregion
    }
}
#endif
