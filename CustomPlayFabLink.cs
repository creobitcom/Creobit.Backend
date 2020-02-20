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

        void IBasicLink.Link(bool forceRelink, Action onComplete, Action onFailure)
        {
            PlayFabClientAPI.LinkCustomID
            (
                new LinkCustomIDRequest()
                {
                    CustomId = CustomId,
                    ForceLink = forceRelink
                },
                result => onComplete(),
                error => onFailure()
            );
        }
    }
}
#endif