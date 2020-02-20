#if CREOBIT_BACKEND_ANDROID && CREOBIT_BACKEND_PLAYFAB && UNITY_ANDROID
using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine;

namespace Creobit.Backend.Auth
{
    public sealed class AndroidDevicePlayFabAuth : PlayFabAuthDecorator
    {
        #region AndroidDevicePlayFabAuth

        public AndroidDevicePlayFabAuth(IPlayFabAuth playFabAuth)
            : base(playFabAuth)
        {
        }

        #endregion
        #region PlayFabAuthDecorator

        protected override void Login(bool doCreateAccount, Action onComplete, Action onFailure)
        {
            PlayFabClientAPI.LoginWithAndroidDeviceID
            (
                new LoginWithAndroidDeviceIDRequest()
                {
                    AndroidDevice = SystemInfo.deviceModel,
                    AndroidDeviceId = SystemInfo.deviceUniqueIdentifier,
                    CreateAccount = doCreateAccount,
                    InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
                    {
                        GetUserAccountInfo = true
                    },
                    OS = SystemInfo.operatingSystem,
                    TitleId = TitleId
                },
                result =>
                {
                    LoginResult = result;

                    onComplete();
                },
                error =>
                {
                    onFailure();
                }
            );
        }

        #endregion
    }
}
#endif