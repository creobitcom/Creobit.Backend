#if CREOBIT_BACKEND_PLAYFAB
using PlayFab;
using PlayFab.ClientModels;
using System;

namespace Creobit.Backend
{
    public sealed class CustomPlayFabAuth : ICustomPlayFabAuth
    {
        #region IAuth

        bool IAuth.IsLoggedIn => PlayFabAuth.IsLoggedIn;

        void IAuth.Login(Action onComplete, Action onFailure)
        {
            PlayFabSettings.TitleId = PlayFabAuth.TitleId;

            try
            {
                PlayFabClientAPI.LoginWithCustomID(
                    new LoginWithCustomIDRequest()
                    {
                        CreateAccount = true,
                        CustomId = CustomId,
                        InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
                        {
                            GetUserAccountInfo = true
                        },
                        TitleId = PlayFabAuth.TitleId
                    },
                    result =>
                    {
                        PlayFabAuth.LoginResult = result;

                        onComplete();
                    },
                    error =>
                    {
                        PlayFabErrorHandler?.Process(error);

                        onFailure();
                    });
            }
            catch (Exception exception)
            {
                ExceptionHandler?.Process(exception);

                onFailure();
            }
        }

        void IAuth.Logout(Action onComplete, Action onFailure) => PlayFabAuth.Logout(onComplete, onFailure);

        #endregion
        #region IPlayFabAuth

        LoginResult IPlayFabAuth.LoginResult
        {
            get => PlayFabAuth.LoginResult;
            set => PlayFabAuth.LoginResult = value;
        }

        string IPlayFabAuth.TitleId => PlayFabAuth.TitleId;

        #endregion
        #region CustomPlayFabAuth

        private readonly IPlayFabAuth PlayFabAuth;
        private readonly string CustomId;

        public CustomPlayFabAuth(IPlayFabAuth playFabAuth, string customId)
        {
            PlayFabAuth = playFabAuth;
            CustomId = customId;
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
#endif
