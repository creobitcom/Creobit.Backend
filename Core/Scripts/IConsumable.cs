using System;

namespace Creobit.Backend
{
    public interface IConsumable
    {
        #region IConsumable

        void Consume(uint count, Action onComplete, Action onFailure);

        #endregion
    }
}
