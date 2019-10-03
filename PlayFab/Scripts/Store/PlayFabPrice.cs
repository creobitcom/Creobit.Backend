#if CREOBIT_BACKEND_PLAYFAB
namespace Creobit.Backend.Store
{
    internal sealed class PlayFabPrice : Price, IPlayFabPrice
    {
        #region IPlayFabPrice

        string IPlayFabPrice.VirtualCurrencyId => VirtualCurrencyId;

        #endregion
        #region PlayFabPrice

        private readonly string VirtualCurrencyId;

        public PlayFabPrice(string id, string currencyCode, uint value, string virtualCurrencyId) : base(id, currencyCode, value)
        {
            VirtualCurrencyId = virtualCurrencyId;
        }

        #endregion
    }
}
#endif
