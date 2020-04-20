#if CREOBIT_BACKEND_PLAYFAB
using PlayFab;
using PlayFab.ClientModels;
using System;

namespace Creobit.Backend.Link
{
    public sealed class CustomPlayFabLink : PlayfabLinkBasic
    {
        #region CustomPlayFabLink

        private readonly string CustomId;

        public CustomPlayFabLink(string customId)
        {
            CustomId = customId;
        }

        #endregion
        #region PlayfabLinkBasic

        protected override bool CanLink(LoginResult login)
        {
            return true;
        }

        protected override void Link(bool forceRelink, Action onComplete, Action<LinkingError> onFailure)
        {
            PlayFabClientAPI.LinkCustomID
            (
                new LinkCustomIDRequest()
                {
                    CustomId = CustomId,
                    ForceLink = forceRelink
                },
                result => onComplete(),
                error => onFailure(LinkingError.Other)
            );
        }

        protected override void Unlink(Action onComplete, Action onFailure)
        {
            var request = new UnlinkCustomIDRequest()
            {
                CustomId = CustomId
            };

            PlayFabClientAPI.UnlinkCustomID
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