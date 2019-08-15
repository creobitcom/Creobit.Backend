#if CREOBIT_BACKEND_GOOGLEPLAYPLAYFAB && CREOBIT_BACKEND_PLAYFAB
namespace Creobit.Backend
{
    public interface IGooglePlayPlayFabAuth : IPlayFabAuth, IGooglePlayAuth
    {
    }
}
#endif
