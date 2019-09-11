#if CREOBIT_BACKEND_GOOGLEPLAY && UNITY_ANDROID
using GooglePlayGames;
using System;

namespace Creobit.Backend.User
{
    public sealed class GooglePlayUser : IGooglePlayUser
    {
        #region IUser

        string IUser.AvatarUrl => PlayGamesPlatform.Instance.GetUserImageUrl();

        string IUser.Name => PlayGamesPlatform.Instance.GetUserDisplayName();

        void IUser.Refresh(Action onComplete, Action onFailure)
        {
            var exception = new NotSupportedException();

            ExceptionHandler.Process(exception);

            onFailure();
        }

        void IUser.SetAvatarUrl(string avatarUrl, Action onComplete, Action onFailure)
        {
            var exception = new NotSupportedException();

            ExceptionHandler.Process(exception);

            onFailure();
        }

        void IUser.SetName(string name, Action onComplete, Action onFailure)
        {
            var exception = new NotSupportedException();

            ExceptionHandler.Process(exception);

            onFailure();
        }

        #endregion
        #region GooglePlayUser

        private IExceptionHandler _exceptionHandler;

        public IExceptionHandler ExceptionHandler
        {
            get => _exceptionHandler ?? Backend.ExceptionHandler.Default;
            set => _exceptionHandler = value;
        }

        #endregion
    }
}
#endif
