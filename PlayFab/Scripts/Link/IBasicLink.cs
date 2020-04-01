#if CREOBIT_BACKEND_PLAYFAB
using PlayFab.ClientModels;
using System;

namespace Creobit.Backend.Link
{
    public interface IBasicLink
    {
        void Link(bool forceRelink, Action onComplete, Action onFailure);
        bool CanLink(LoginResult login);
    }
}
#endif