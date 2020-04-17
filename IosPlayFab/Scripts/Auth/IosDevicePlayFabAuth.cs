#if CREOBIT_BACKEND_IOS && CREOBIT_BACKEND_PLAYFAB && UNITY_IOS
using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine;

namespace Creobit.Backend.Auth
{
    public sealed class IosDevicePlayFabAuth : PlayFabAuthDecorator
    {
        public IosDevicePlayFabAuth(IPlayFabAuth playFabAuth)
            : base(playFabAuth)
        {
        }

        #region PlayFabAuthDecorator

        protected override void Login(bool doCreateAccount, Action onComplete, Action onFailure)
        {
            PlayFabClientAPI.LoginWithIOSDeviceID
            (
                new LoginWithIOSDeviceIDRequest()
                {
                    CreateAccount = doCreateAccount,
                    DeviceId = SystemInfo.deviceUniqueIdentifier,
                    DeviceModel = SystemInfo.deviceModel,
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
                error => onFailure()
            );
        }

        #endregion
    }
}
#endif