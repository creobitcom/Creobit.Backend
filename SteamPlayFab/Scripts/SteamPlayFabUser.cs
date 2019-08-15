#if UNITY_EDITOR || UNITY_STANDALONE
#define ENABLED
#endif
#if ENABLED
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
#else
using System;

namespace Creobit.Backend
{
    public sealed class SteamPlayFabUser : IPlayFabUser, ISteamUser
    {
        string IUser.UserName => throw new NotSupportedException();

        void IUser.SetUserName(string userName, Action onComplete, Action onFailure) => throw new NotSupportedException();

        public SteamPlayFabUser(IPlayFabUser playFabUser, ISteamUser steamUser)
        {
        }
    }
}
#endif