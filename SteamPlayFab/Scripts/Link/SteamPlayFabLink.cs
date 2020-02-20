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

        void IBasicLink.Link(bool forceRelink, Action onComplete, Action onFailure)
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
                    onComplete();
                },
                error =>
                {
                    SteamAuth.DestroyAuthSessionTicket(authSessionTicket);

                    onFailure();
                }
            );
        }
    }
}
#endif