#if CREOBIT_BACKEND_PLAYFAB && CREOBIT_BACKEND_STEAM && UNITY_STANDALONE
using System;

namespace Creobit.Backend
{
    public sealed class SteamPlayFabUser : IPlayFabUser, ISteamUser
    {
        #region IUser

        string IUser.UserName => string.IsNullOrWhiteSpace(PlayFabUser.UserName) ? SteamUser.UserName : PlayFabUser.UserName;

        void IUser.SetUserName(string userName, Action onComplete, Action onFailure) => PlayFabUser.SetUserName(userName, onComplete, onFailure);

        #endregion
        #region SteamPlayFabUser

        private readonly IPlayFabUser PlayFabUser;
        private readonly ISteamUser SteamUser;

        public SteamPlayFabUser(IPlayFabUser playFabUser, ISteamUser steamUser)
        {
            PlayFabUser = playFabUser;
            SteamUser = steamUser;
        }

        #endregion
    }
}
#endif
