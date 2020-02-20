#if CREOBIT_BACKEND_PLAYFAB
using PlayFab;
using PlayFab.ClientModels;
using System;

namespace Creobit.Backend.Auth
{
    public sealed class PlayFabAuth : IPlayFabAuth
    {
        #region IAuth

        bool IAuth.IsLoggedIn => PlayFabClientAPI.IsClientLoggedIn();

        void IAuth.Login(bool doCreateAccount, Action onComplete, Action onFailure)
        {
            var exception = new NotSupportedException();

            ExceptionHandler.Process(exception);

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

        private IExceptionHandler _exceptionHandler;

        public PlayFabAuth(string titleId)
        {
            TitleId = titleId;
            PlayFabSettings.TitleId = titleId;
        }

        public IExceptionHandler ExceptionHandler
        {
            get => _exceptionHandler ?? Backend.ExceptionHandler.Default;
            set => _exceptionHandler = value;
        }

        #endregion
    }
}
#endif
