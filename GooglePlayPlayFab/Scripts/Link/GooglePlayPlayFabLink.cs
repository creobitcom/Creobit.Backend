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

        private void Link(string serverAuthCode, bool forceRelink, Action onComplete, Action<LinkingError> onFailure)
        {
            PlayFabClientAPI.LinkGoogleAccount
            (
                new LinkGoogleAccountRequest()
                {
                    ForceLink = forceRelink,
                    ServerAuthCode = serverAuthCode
                },
                result => onComplete?.Invoke(),
                error => onFailure?.Invoke(LinkingError.Other)
            );
        }

        #region IBasicLink

        bool IBasicLink.CanLink(LoginResult login)
        {
            var payload = login?.InfoResultPayload;
            var accountInfo = payload?.AccountInfo;
            var googleInfo = accountInfo?.GoogleInfo;

            return string.IsNullOrWhiteSpace(googleInfo?.GoogleId);
        }

        void IBasicLink.Link(bool forceRelink, Action onComplete, Action<LinkingError> onFailure)
        {
            GoogleAuth.GetServerAuthCode
            (
                serverAuthCode => Link(serverAuthCode, forceRelink, onComplete, onFailure),
                () => onFailure?.Invoke(LinkingError.CanceledByUser)
            );
        }

        void IBasicLink.Unlink(Action onComplete, Action onFailure)
        {
            var request = new UnlinkGoogleAccountRequest();
            PlayFabClientAPI.UnlinkGoogleAccount(request, result => onComplete?.Invoke(), error => onFailure?.Invoke());
        }

        #endregion
    }
}
#endif