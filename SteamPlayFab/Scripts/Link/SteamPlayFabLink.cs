#if CREOBIT_BACKEND_PLAYFAB && CREOBIT_BACKEND_STEAM && UNITY_STANDALONE
using Creobit.Backend.Auth;
using PlayFab;
using PlayFab.ClientModels;
using System;

namespace Creobit.Backend.Link
{
    public sealed class SteamPlayfabLink : IBasicLink
    {
        private readonly ISteamAuth SteamAuth;

        public SteamPlayfabLink(ISteamAuth steamAuth)
        {
            SteamAuth = steamAuth;
        }

        bool IBasicLink.CanLink(LoginResult login)
        {
            var payload = login?.InfoResultPayload;
            var accountInfo = payload?.AccountInfo;
            var steamInfo = accountInfo?.SteamInfo;

            return string.IsNullOrWhiteSpace(steamInfo?.SteamId);
        }

        void IBasicLink.Link(bool forceRelink, Action onComplete, Action<LinkingError> onFailure)
        {
            var authSessionTicket = SteamAuth.CreateAuthSessionTicket();
            PlayFabClientAPI.LinkSteamAccount
            (
                new LinkSteamAccountRequest()
                {
                    ForceLink = forceRelink,
                    SteamTicket = authSessionTicket
                },
                result =>
                {
                    SteamAuth.DestroyAuthSessionTicket(authSessionTicket);
                    onComplete?.Invoke();
                },
                error =>
                {
                    SteamAuth.DestroyAuthSessionTicket(authSessionTicket);
                    onFailure?.Invoke(LinkingError.Other);
                }
            );
        }

        void IBasicLink.Unlink(Action onComplete, Action onFailure)
        {
            var request = new UnlinkSteamAccountRequest();
            PlayFabClientAPI.UnlinkSteamAccount(request, result => onComplete?.Invoke(), error => onFailure?.Invoke());
        }
    }
}
#endif