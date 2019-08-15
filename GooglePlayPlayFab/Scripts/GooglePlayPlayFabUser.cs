#if UNITY_ANDROID && !UNITY_EDITOR
#define ENABLED
#endif
#if ENABLED
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
#else
using System;

namespace Creobit.Backend
{
    public sealed class GooglePlayPlayFabUser : IPlayFabUser, IGooglePlayUser
    {
        string IUser.UserName => throw new NotSupportedException();

        void IUser.SetUserName(string userName, Action onComplete, Action onFailure) => throw new NotSupportedException();

        public GooglePlayPlayFabUser(IPlayFabUser playFabUser, IGooglePlayUser googlePlayUser)
        {
        }
    }
}
#endif