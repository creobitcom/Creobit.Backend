#if CREOBIT_BACKEND_PLAYFAB
using PlayFab;
using PlayFab.ClientModels;
using System;

namespace Creobit.Backend.Link
{
    public sealed class CustomPlayFabLink : IBasicLink
    {
        private readonly string CustomId;

        public CustomPlayFabLink(string customId)
        {
            CustomId = customId;
        }

        bool IBasicLink.CanLink(LoginResult login)
        {
            return true;
        }

        void IBasicLink.Link(bool forceRelink, Action onComplete, Action<LinkingError> onFailure)
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

        void IBasicLink.Unlink(Action onComplete, Action onFailure)
        {
            var request = new UnlinkCustomIDRequest()
            {
                CustomId = CustomId
            };
            PlayFabClientAPI.UnlinkCustomID(request, result => onComplete?.Invoke(), error => onFailure?.Invoke());
        }
    }
}
#endif