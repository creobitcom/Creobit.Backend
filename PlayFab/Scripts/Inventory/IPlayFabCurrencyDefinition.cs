#if CREOBIT_BACKEND_PLAYFAB
using PlayFab.ClientModels;

namespace Creobit.Backend.Inventory
{
    public interface IPlayFabCurrencyDefinition : ICurrencyDefinition<IPlayFabCurrencyInstance>
    {
        #region IPlayFabCurrencyDefinition

        VirtualCurrencyRechargeTime NativeVirtualCurrencyRechargeTime
        {
            get;
        }

        #endregion
    }
}
#endif
