#if CREOBIT_BACKEND_STEAM && UNITY_STANDALONE
using Steamworks;
using System;

namespace Creobit.Backend.User
{
    public sealed class SteamUser : ISteamUser
    {
        #region IUser

        string IUser.AvatarUrl
        {
            get
            {
                var exception = new NotSupportedException();

                ExceptionHandler.Process(exception);

                return string.Empty;
            }
        }

        string IUser.Name => SteamClient.Name;

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
        #region SteamUser

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
