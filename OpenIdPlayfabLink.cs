#if CREOBIT_BACKEND_PLAYFAB
using Creobit.Backend.Auth;
using PlayFab;
using PlayFab.ClientModels;
using System;


namespace Creobit.Backend.Link
{
    public sealed class OpenIdPlayfabLink : IBasicLink
    {
        #region IBasicLink

        void IBasicLink.Link(bool forceRelink, Action onComplete, Action onFailure)
        {
            OpenIdProvider.RequestToken(token => Link(token, forceRelink, onComplete, onFailure));
        }

        #endregion

        private readonly IOpenIdProvider OpenIdProvider;

        public OpenIdPlayfabLink(IOpenIdProvider openIdProvider)
        {
            OpenIdProvider = openIdProvider;
        }

        private void Link(string idToken, bool forceRelink, Action onComplete, Action onFailure)
        {
            var request = new LinkOpenIdConnectRequest()
            {
                IdToken = idToken,
                ConnectionId = OpenIdProvider.Id,
                ForceLink = forceRelink
            };

            PlayFabClientAPI.LinkOpenIdConnect(request, result => onComplete?.Invoke(), error => onFailure?.Invoke());
        }
    }
}
#endif