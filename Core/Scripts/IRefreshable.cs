using System;

namespace Creobit.Backend
{
    public interface IRefreshable
    {
        #region IRefreshable

        void Refresh(Action onComplete, Action onFailure);

        #endregion
    }
}
