#if CREOBIT_BACKEND_GOOGLEPLAY
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
#endif
