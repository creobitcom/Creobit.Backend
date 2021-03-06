﻿#if CREOBIT_BACKEND_GOOGLEPLAY && CREOBIT_BACKEND_PLAYFAB && UNITY_ANDROID
using PlayFab;
using PlayFab.ClientModels;
using System;

namespace Creobit.Backend.Auth
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
                                    PlayFabErrorHandler.Process(error);

                                    onFailure();
                                });
                        }
                        catch (Exception exception)
                        {
                            ExceptionHandler.Process(exception);

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

        private IExceptionHandler _exceptionHandler;
        private IPlayFabErrorHandler _playFabErrorHandler;

        public GooglePlayPlayFabAuth(IPlayFabAuth playFabAuth, IGooglePlayAuth googlePlayAuth)
        {
            PlayFabAuth = playFabAuth;
            GooglePlayAuth = googlePlayAuth;
        }

        public IExceptionHandler ExceptionHandler
        {
            get => _exceptionHandler ?? Backend.ExceptionHandler.Default;
            set => _exceptionHandler = value;
        }

        public IPlayFabErrorHandler PlayFabErrorHandler
        {
            get => _playFabErrorHandler ?? Backend.PlayFabErrorHandler.Default;
            set => _playFabErrorHandler = value;
        }

        #endregion
    }
}
#endif
