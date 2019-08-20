#if CREOBIT_BACKEND_GOOGLEPLAY && UNITY_ANDROID
using GooglePlayGames;
using System;

namespace Creobit.Backend
{
    public sealed class GooglePlayUser : IGooglePlayUser
    {
        #region IUser

        string IUser.Name => PlayGamesPlatform.Instance.GetUserDisplayName();

        void IUser.Refresh(Action onComplete, Action onFailure)
        {
            var exception = new NotSupportedException();

            ExceptionHandler?.Process(exception);

            onFailure();
        }

        void IUser.SetName(string name, Action onComplete, Action onFailure)
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
