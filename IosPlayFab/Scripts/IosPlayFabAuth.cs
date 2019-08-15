#if CREOBIT_BACKEND_IOSPLAYFAB && CREOBIT_BACKEND_PLAYFAB
using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine;

namespace Creobit.Backend
{
    public sealed class IosPlayFabAuth : IIosPlayFabAuth
    {
        #region IAuth

        bool IAuth.IsLoggedIn => PlayFabAuth.IsLoggedIn;

        void IAuth.Login(Action onComplete, Action onFailure)
        {
            PlayFabSettings.TitleId = PlayFabAuth.TitleId;

            try
            {
                PlayFabClientAPI.LoginWithIOSDeviceID(
                    new LoginWithIOSDeviceIDRequest()
                    {
                        CreateAccount = true,
                        DeviceId = SystemInfo.deviceUniqueIdentifier,
                        DeviceModel = SystemInfo.deviceModel,
                        InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
                        {
                            GetUserAccountInfo = true
                        },
                        OS = SystemInfo.operatingSystem,
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
        #region IosPlayFabAuth

        private readonly IPlayFabAuth PlayFabAuth;

        public IosPlayFabAuth(IPlayFabAuth playFabAuth)
        {
            PlayFabAuth = playFabAuth;
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
