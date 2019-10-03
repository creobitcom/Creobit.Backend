using System.Collections.Generic;

namespace Creobit.Backend.Store
{
    public interface IStore : IRefreshable
    {
        #region IStore

        IEnumerable<IProduct> Products
        {
            get;
        }

        IEnumerable<ISubscription> Subscriptions
        {
            get;
        }

        #endregion
    }
}
