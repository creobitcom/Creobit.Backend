#if UNITY_ANDROID && !UNITY_EDITOR
#define ENABLED
#endif
#if ENABLED
using GooglePlayGames;
using System;

namespace Creobit.Backend
{
    public sealed class GooglePlayUser : IGooglePlayUser
    {
        #region IUser

        string IUser.UserName => PlayGamesPlatform.Instance.GetUserDisplayName();

        void IUser.SetUserName(string userName, Action onComplete, Action onFailure)
        {
            var exception = new NotSupportedException();

            ExceptionHandler?.Process(exception);

            onFailure();
        }

        #endregion
        #region GooglePlayUser

        public IExceptionHandler ExceptionHandler
        {
            get;
            set;
        } = Backend.ExceptionHandler.Default;

        #endregion
    }
}
#else
using System;

namespace Creobit.Backend
{
    public sealed class GooglePlayUser : IGooglePlayUser
    {
        string IUser.UserName => throw new NotSupportedException();

        void IUser.SetUserName(string userName, Action onComplete, Action onFailure) => throw new NotSupportedException();
    }
}
#endif