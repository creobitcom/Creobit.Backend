#if CREOBIT_BACKEND_PLAYFAB
namespace Creobit.Backend.Store
{
    public interface IPlayFabPrice : IPrice
    {
        #region IPlayFabPrice

        string VirtualCurrencyId
        {
            get;
        }

        #endregion
    }
}
#endif
