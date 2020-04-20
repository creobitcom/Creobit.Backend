#if CREOBIT_BACKEND_PLAYFAB
using PlayFab.ClientModels;
using System;

namespace Creobit.Backend.Link
{
    public abstract class PlayfabLinkBasic : IBasicLink
    {
        #region LinkBasic

        protected IPlayFabErrorHandler PlayFabErrorHandler => Backend.PlayFabErrorHandler.Default;

        protected abstract bool CanLink(LoginResult login);

        protected abstract void Link(bool forceRelink, Action onComplete, Action<LinkingError> onFailure);

        protected abstract void Unlink(Action onComplete, Action onFailure);

        #endregion
        #region IBasicLink

        bool IBasicLink.CanLink(LoginResult login)
        {
            return CanLink(login);
        }

        void IBasicLink.Link(bool forceRelink, Action onComplete, Action<LinkingError> onFailure)
        {
            Link(forceRelink, onComplete, onFailure);
        }

        void IBasicLink.Unlink(Action onComplete, Action onFailure)
        {
            Unlink(onComplete, onFailure);
        }

        #endregion
    }
}
#endif