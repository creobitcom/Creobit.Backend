#if CREOBIT_BACKEND_STEAM && UNITY_STANDALONE
using Steamworks;
using System;

namespace Creobit.Backend
{
    public sealed class SteamUser : ISteamUser
    {
        #region IUser

        string IUser.AvatarUrl
        {
            get
            {
                var exception = new NotSupportedException();

                ExceptionHandler?.Process(exception);

                return string.Empty;
            }
        }

        string IUser.Name => SteamClient.Name;

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
        #region SteamUser

        public IExceptionHandler ExceptionHandler
        {
            get;
            set;
        } = Backend.ExceptionHandler.Default;

        #endregion
    }
}
#endif
