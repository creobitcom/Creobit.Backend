#if CREOBIT_BACKEND_STEAM && UNITY_STANDALONE
using System;

namespace Creobit.Backend.Auth
{
    public sealed class SteamIsNotRunningException : Exception
    {
        #region Exception

        public override string Message => "SteamApi_Init returned false. Steam isn't running, couldn't find Steam, AppId is ureleased, Don't own AppId.";

        #endregion
    }
}
#endif
