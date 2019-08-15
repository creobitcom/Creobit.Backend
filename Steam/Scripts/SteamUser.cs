#if CREOBIT_BACKEND_STEAM
using Steamworks;
using System;

namespace Creobit.Backend
{
    public sealed class SteamUser : ISteamUser
    {
        #region IUser

        string IUser.UserName => SteamClient.Name;

        void IUser.SetUserName(string userName, Action onComplete, Action onFailure)
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
