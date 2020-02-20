#if CREOBIT_BACKEND_GOOGLEPLAY && CREOBIT_BACKEND_PLAYFAB && UNITY_ANDROID
using Creobit.Backend.Auth;
using PlayFab;
using PlayFab.ClientModels;
using System;

namespace Creobit.Backend.Link
{
    public sealed class GooglePlayPlayFabLink : IBasicLink
    {
        private readonly IGooglePlayAuth GoogleAuth;

        public GooglePlayPlayFabLink(IGooglePlayAuth googleAuth)
        {
            GoogleAuth = googleAuth;
        }

        private void Link(string serverAuthCode, bool forceRelink, Action onComplete, Action onFailure)
        {
            PlayFabClientAPI.LinkGoogleAccount
            (
                new LinkGoogleAccountRequest()
                {
                    ForceLink = forceRelink,
                    ServerAuthCode = serverAuthCode
                },
                result => onComplete(),
                error => onFailure()
            );
        }

        #region IBasicLink

        void IBasicLink.Link(bool forceRelink, Action onComplete, Action onFailure)
        {
            GoogleAuth.GetServerAuthCode
            (
                serverAuthCode => Link(serverAuthCode, forceRelink, onComplete, onFailure),
                onFailure
            );
        }

        #endregion
    }
}
#endif