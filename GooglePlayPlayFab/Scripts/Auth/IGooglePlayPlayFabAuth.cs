#if CREOBIT_BACKEND_GOOGLEPLAY && CREOBIT_BACKEND_PLAYFAB && UNITY_ANDROID
namespace Creobit.Backend.Auth
{
    public interface IGooglePlayPlayFabAuth : IPlayFabAuth, IGooglePlayAuth
    {
    }
}
#endif
