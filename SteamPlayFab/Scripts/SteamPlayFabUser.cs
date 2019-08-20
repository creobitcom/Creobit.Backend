#if CREOBIT_BACKEND_PLAYFAB && CREOBIT_BACKEND_STEAM && UNITY_STANDALONE
using System;

namespace Creobit.Backend
{
    public sealed class SteamPlayFabUser : IPlayFabUser, ISteamUser
    {
        #region IUser

        string IUser.AvatarUrl => PlayFabUser.AvatarUrl;

        string IUser.Name => string.IsNullOrWhiteSpace(PlayFabUser.Name) ? SteamUser.Name : PlayFabUser.Name;

        void IUser.Refresh(Action onComplete, Action onFailure) =>  PlayFabUser.Refresh(onComplete, onFailure);

        void IUser.SetAvatarUrl(string avatarUrl, Action onComplete, Action onFailure) => PlayFabUser.SetAvatarUrl(avatarUrl, onComplete, onFailure);

        void IUser.SetName(string name, Action onComplete, Action onFailure) => PlayFabUser.SetName(name, onComplete, onFailure);

        #endregion
        #region IPlayFabUser

        string IPlayFabUser.Id => PlayFabUser.Id;

        bool IPlayFabUser.IsNewlyCreated => PlayFabUser.IsNewlyCreated;

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
