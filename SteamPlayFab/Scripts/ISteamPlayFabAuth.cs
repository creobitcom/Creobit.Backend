﻿#if CREOBIT_BACKEND_PLAYFAB && CREOBIT_BACKEND_STEAM && UNITY_STANDALONE
namespace Creobit.Backend
{
    public interface ISteamPlayFabAuth : IPlayFabAuth, ISteamAuth
    {
    }
}
#endif
