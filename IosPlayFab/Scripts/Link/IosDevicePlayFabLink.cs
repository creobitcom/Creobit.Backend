#if CREOBIT_BACKEND_IOS && CREOBIT_BACKEND_PLAYFAB && UNITY_IOS
using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine;

namespace Creobit.Backend.Link
{
    public sealed class IosDevicePlayFabLink : IBasicLink
    {
        void IBasicLink.Link(bool forceRelink, Action onComplete, Action onFailure)
        {
            PlayFabClientAPI.LinkIOSDeviceID
            (
                new LinkIOSDeviceIDRequest()
                {
                    DeviceId = SystemInfo.deviceUniqueIdentifier,
                    DeviceModel = SystemInfo.deviceModel,
                    ForceLink = forceRelink,
                    OS = SystemInfo.operatingSystem
                },
                result => onComplete(),
                error => onFailure()
            );
        }
    }
}
#endif