#if UNITY_ANDROID && !UNITY_EDITOR
#define ENABLED
#endif
#if ENABLED
using PlayFab;
using PlayFab.ClientModels;
using System;

namespace Creobit.Backend
{
    public sealed class GooglePlayPlayFabAuth : IGooglePlayPlayFabAuth
    {
        #region IAuth

        bool IAuth.IsLoggedIn => PlayFabAuth.IsLoggedIn && GooglePlayAuth.IsLoggedIn;

        void IAuth.Login(Action onComplete, Action onFailure)
        {
            GooglePlayLogin();

            void GooglePlayLogin()
            {
                GooglePlayAuth.Login(PlayFabLogin, onFailure);
            }

            void PlayFabLogin()
            {
                GooglePlayAuth.GetServerAuthCode(
                    serverAuthCode =>
                    {
                        PlayFabSettings.TitleId = PlayFabAuth.TitleId;

                        try
                        {
                            PlayFabClientAPI.LoginWithGoogleAccount(
                                new LoginWithGoogleAccountRequest()
                                {
                                    CreateAccount = true,
                                    InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
                                    {
                                        GetUserAccountInfo = true
                                    },
                                    ServerAuthCode = serverAuthCode,
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
                    }, onFailure);
            }
        }

        void IAuth.Logout(Action onComplete, Action onFailure)
        {
            PlayFabLogout();

            void PlayFabLogout()
            {
                PlayFabAuth.Logout(GooglePlayLogout, onFailure);
            }

            void GooglePlayLogout()
            {
                GooglePlayAuth.Logout(onComplete, onFailure);
            }
        }

        #endregion
        #region IPlayFabAuth

        LoginResult IPlayFabAuth.LoginResult
        {
            get => PlayFabAuth.LoginResult;
            set => PlayFabAuth.LoginResult = value;
        }

        string IPlayFabAuth.TitleId => PlayFabAuth.TitleId;

        #endregion
        #region IGooglePlayAuth

        void IGooglePlayAuth.GetServerAuthCode(Action<string> onComplete, Action onFailure) => GooglePlayAuth.GetServerAuthCode(onComplete, onFailure);

        #endregion
        #region GooglePlayPlayFabAuth

        private readonly IPlayFabAuth PlayFabAuth;
        private readonly IGooglePlayAuth GooglePlayAuth;

        public GooglePlayPlayFabAuth(IPlayFabAuth playFabAuth, IGooglePlayAuth googlePlayAuth)
        {
            PlayFabAuth = playFabAuth;
            GooglePlayAuth = googlePlayAuth;
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
#else
using PlayFab.ClientModels;
using System;

namespace Creobit.Backend
{
    public sealed class GooglePlayPlayFabAuth : IGooglePlayPlayFabAuth
    {
        bool IAuth.IsLoggedIn => throw new NotSupportedException();

        void IAuth.Login(Action onComplete, Action onFailure) => throw new NotSupportedException();

        void IAuth.Logout(Action onComplete, Action onFailure) => throw new NotSupportedException();

        LoginResult IPlayFabAuth.LoginResult
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        string IPlayFabAuth.TitleId => throw new NotSupportedException();

        void IGooglePlayAuth.GetServerAuthCode(Action<string> onComplete, Action onFailure) => throw new NotSupportedException();

        public GooglePlayPlayFabAuth(IPlayFabAuth playFabAuth, IGooglePlayAuth googlePlayAuth)
        {
        }
    }
}
#endif