#if CREOBIT_BACKEND_PLAYFAB && CREOBIT_BACKEND_STEAM
namespace Creobit.Backend
{
    public interface ISteamPlayFabAuth : IPlayFabAuth, ISteamAuth
    {
    }
}
#endif
