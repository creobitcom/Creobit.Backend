#if CREOBIT_BACKEND_PLAYFAB && CREOBIT_BACKEND_STEAM && UNITY_STANDALONE
using Creobit.Backend.Auth;
using PlayFab;
using PlayFab.ClientModels;
using System;

namespace Creobit.Backend.Link
{
    public sealed class SteamPlayfabLink : PlayfabLinkBasic
    {
        #region SteamPlayfabLink

        private const int MaxLoginAttempts = 3;

        private readonly ISteamAuth SteamAuth;

        public SteamPlayfabLink(ISteamAuth steamAuth)
        {
            SteamAuth = steamAuth;
        }

        #endregion
        #region PlayfabLinkBasic

        protected override bool CanLink(LoginResult login)
        {
            var payload = login?.InfoResultPayload;
            var accountInfo = payload?.AccountInfo;
            var steamInfo = accountInfo?.SteamInfo;

            return string.IsNullOrWhiteSpace(steamInfo?.SteamId);
        }

        protected override void Link(bool forceRelink, Action onComplete, Action<LinkingError> onFailure)
        {
            SteamLink(MaxLoginAttempts);

            void SteamLink(int attemptsRemaining)
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
                        PlayFabErrorHandler.Process(error);
                        SteamAuth.DestroyAuthSessionTicket(authSessionTicket);

                        if (error.Error == PlayFabErrorCode.InvalidSteamTicket)
                        {
                            SteamLink(--attemptsRemaining);
                        }
                        else
                        {
                            onFailure?.Invoke(LinkingError.Other);
                        }
                    }
                );
            }
        }

        protected override void Unlink(Action onComplete, Action onFailure)
        {
            var request = new UnlinkSteamAccountRequest();
            PlayFabClientAPI.UnlinkSteamAccount
            (
                request,
                result => onComplete?.Invoke(),
                error =>
                {
                    PlayFabErrorHandler.Process(error);
                    onFailure?.Invoke();
                }
            );
        }

        #endregion
    }
}
#endif