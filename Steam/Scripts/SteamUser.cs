#if UNITY_EDITOR || UNITY_STANDALONE
#define ENABLED
#endif
#if ENABLED
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
#else
using System;

namespace Creobit.Backend
{
    public sealed class SteamUser : ISteamUser
    {
        string IUser.UserName => throw new NotSupportedException();

        void IUser.SetUserName(string userName, Action onComplete, Action onFailure) => throw new NotSupportedException();
    }
}
#endif