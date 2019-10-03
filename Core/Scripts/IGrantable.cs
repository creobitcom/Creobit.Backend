using System;

namespace Creobit.Backend
{
    public interface IGrantable
    {
        #region IGrantable

        void Grant(uint count, Action onComplete, Action onFailure);

        #endregion
    }
}
