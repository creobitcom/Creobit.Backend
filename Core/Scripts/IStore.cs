using System;
using System.Collections.Generic;

namespace Creobit.Backend
{
    public interface IStore
    {
        #region IStore

        IEnumerable<IProduct> Products
        {
            get;
        }

        void LoadProducts(Action onComplete, Action onFailure);

        #endregion
    }
}
