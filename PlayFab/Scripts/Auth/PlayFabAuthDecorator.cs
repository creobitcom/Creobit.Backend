#if CREOBIT_BACKEND_PLAYFAB
using PlayFab.ClientModels;
using System;

namespace Creobit.Backend.Auth
{
    public abstract class PlayFabAuthDecorator : IPlayFabAuth
    {
        #region PlayFabAuthDecorator

        private readonly IPlayFabAuth PlayFabAuth;

        protected PlayFabAuthDecorator(IPlayFabAuth playFabAuth)
        {
            PlayFabAuth = playFabAuth;
        }

        protected abstract void Login(bool doCreateAccount, Action onComplete, Action onFailure);

        #endregion
        #region IPlayFabAuth

        public LoginResult LoginResult
        {
            get => PlayFabAuth.LoginResult;
            set => PlayFabAuth.LoginResult = value;
        }

        public string TitleId => PlayFabAuth.TitleId;

        bool IAuth.IsLoggedIn => PlayFabAuth.IsLoggedIn;

        void IAuth.Login(bool doCreateAccount, Action onComplete, Action onFailure)
        {
            Login(doCreateAccount, onComplete, onFailure);
        }

        void IAuth.Logout(Action onComplete, Action onFailure)
        {
            PlayFabAuth.Logout(onComplete, onFailure);
        }

        #endregion
    }
}
#endif