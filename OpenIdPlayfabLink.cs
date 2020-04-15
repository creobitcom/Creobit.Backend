#if CREOBIT_BACKEND_PLAYFAB
using Creobit.Backend.Auth;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Linq;

namespace Creobit.Backend.Link
{
    public sealed class OpenIdPlayfabLink : IBasicLink
    {
        #region IBasicLink

        bool IBasicLink.CanLink(LoginResult login)
        {
            var payload = login?.InfoResultPayload;
            var accountInfo = payload?.AccountInfo;
            var openIdAccounts = accountInfo?.OpenIdInfo;

            return openIdAccounts == null || !openIdAccounts.Any(any => any.ConnectionId == OpenIdProvider.Id);
        }

        void IBasicLink.Link(bool forceRelink, Action onComplete, Action<LinkingError> onFailure)
        {
            OpenIdProvider.RequestToken(token => Link(token, forceRelink, onComplete, onFailure));
        }

        void IBasicLink.Unlink(Action onComplete, Action onFailure)
        {
            OpenIdProvider.RequestToken(token => Unlink(token, onComplete, onFailure));
        }

        #endregion

        private readonly IOpenIdProvider OpenIdProvider;

        public OpenIdPlayfabLink(IOpenIdProvider openIdProvider)
        {
            OpenIdProvider = openIdProvider;
        }

        private void Link(string idToken, bool forceRelink, Action onComplete, Action<LinkingError> onFailure)
        {
            if (string.IsNullOrWhiteSpace(idToken))
            {
                onFailure?.Invoke(LinkingError.CanceledByUser);
                return;
            }

            var request = new LinkOpenIdConnectRequest()
            {
                IdToken = idToken,
                ConnectionId = OpenIdProvider.Id,
                ForceLink = forceRelink
            };

            PlayFabClientAPI.LinkOpenIdConnect(request, result => onComplete?.Invoke(), error => ProcessError(error, onFailure));
        }

        private void ProcessError(PlayFabError error, Action<LinkingError> onFailure)
        {
            PlayFabErrorHandler.Default.Process(error);
            var linkError = GetLinkError(error);
            onFailure?.Invoke(linkError);
        }

        private LinkingError GetLinkError(PlayFabError error)
        {
            switch (error.Error)
            {
                case PlayFabErrorCode.LinkedIdentifierAlreadyClaimed:
                    return LinkingError.AlreadyLinked;

                default:
                    return LinkingError.Other;
            }
        }

        private void Unlink(string token, Action onComplete, Action onFailure)
        {
            var request = new UninkOpenIdConnectRequest()
            {
                ConnectionId = token
            };
            PlayFabClientAPI.UnlinkOpenIdConnect(request, result => onComplete?.Invoke(), error => onFailure?.Invoke());
        }
    }
}
#endif