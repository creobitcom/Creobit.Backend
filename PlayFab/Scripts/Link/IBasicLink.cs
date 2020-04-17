#if CREOBIT_BACKEND_PLAYFAB
using PlayFab.ClientModels;
using System;

namespace Creobit.Backend.Link
{
    public interface IBasicLink
    {
        bool CanLink(LoginResult login);
        void Link(bool forceRelink, Action onComplete, Action<LinkingError> onFailure);
        void Unlink(Action onComplete, Action onFailure);
    }
}
#endif