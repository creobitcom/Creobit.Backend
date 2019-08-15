#if CREOBIT_BACKEND_GOOGLEPLAYPLAYFAB && CREOBIT_BACKEND_PLAYFAB
using System;

namespace Creobit.Backend
{
    public sealed class GooglePlayPlayFabUser : IPlayFabUser, IGooglePlayUser
    {
        #region IUser

        string IUser.UserName => string.IsNullOrWhiteSpace(PlayFabUser.UserName) ? GooglePlayUser.UserName : PlayFabUser.UserName;

        void IUser.SetUserName(string userName, Action onComplete, Action onFailure) => PlayFabUser.SetUserName(userName, onComplete, onFailure);

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
