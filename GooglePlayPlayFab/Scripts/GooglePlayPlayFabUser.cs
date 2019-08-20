#if CREOBIT_BACKEND_GOOGLEPLAY && CREOBIT_BACKEND_PLAYFAB && UNITY_ANDROID
using System;

namespace Creobit.Backend
{
    public sealed class GooglePlayPlayFabUser : IPlayFabUser, IGooglePlayUser
    {
        #region IUser

        string IUser.AvatarUrl => string.IsNullOrWhiteSpace(PlayFabUser.AvatarUrl) ? GooglePlayUser.AvatarUrl : PlayFabUser.AvatarUrl;

        string IUser.Name => string.IsNullOrWhiteSpace(PlayFabUser.Name) ? GooglePlayUser.Name : PlayFabUser.Name;

        void IUser.Refresh(Action onComplete, Action onFailure) => PlayFabUser.Refresh(onComplete, onFailure);

        void IUser.SetName(string name, Action onComplete, Action onFailure) => PlayFabUser.SetName(name, onComplete, onFailure);

        #endregion
        #region GooglePlayPlayFabUser

        private readonly IPlayFabUser PlayFabUser;
        private readonly IGooglePlayUser GooglePlayUser;

        public GooglePlayPlayFabUser(IPlayFabUser playFabUser, IGooglePlayUser googlePlayUser)
        {
            PlayFabUser = playFabUser;
            GooglePlayUser = googlePlayUser;
        }

        #endregion
    }
}
#endif
