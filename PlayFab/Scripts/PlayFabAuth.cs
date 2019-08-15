using PlayFab;
using PlayFab.ClientModels;
using System;

namespace Creobit.Backend
{
    public sealed class PlayFabAuth : IPlayFabAuth
    {
        #region IAuth

        bool IAuth.IsLoggedIn => PlayFabClientAPI.IsClientLoggedIn();

        void IAuth.Login(Action onComplete, Action onFailure)
        {
            var exception = new NotSupportedException();

            ExceptionHandler?.Process(exception);

            onFailure();
        }

        void IAuth.Logout(Action onComplete, Action onFailure)
        {
            if (PlayFabClientAPI.IsClientLoggedIn())
            {
                ((IPlayFabAuth)this).LoginResult = null;

                PlayFabClientAPI.ForgetAllCredentials();
            }

            onComplete();
        }

        #endregion
        #region IPlayFabAuth

        LoginResult IPlayFabAuth.LoginResult
        {
            get;
            set;
        }

        string IPlayFabAuth.TitleId => TitleId;

        #endregion
        #region PlayFabAuth

        private readonly string TitleId;

        public PlayFabAuth(string titleId)
        {
            TitleId = titleId;
        }

        public IExceptionHandler ExceptionHandler
        {
            get;
            set;
        } = Backend.ExceptionHandler.Default;

        public IPlayFabErrorHandler PlayFabErrorHandler
        {
            get;
            set;
        } = Backend.PlayFabErrorHandler.Default;

        #endregion
    }
}