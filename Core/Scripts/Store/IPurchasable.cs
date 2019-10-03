using System;

namespace Creobit.Backend.Store
{
    public interface IPurchasable
    {
        #region IPurchasable

        IPrice Price
        {
            get;
        }

        void Purchase(Action onComplete, Action onFailure);

        #endregion
    }
}
