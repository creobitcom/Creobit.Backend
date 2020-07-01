#if CREOBIT_BACKEND_UNITY
using System.Collections.Generic;
using System;

namespace Creobit.Backend.Store
{
    public interface IUnityStore : IStore
    {
        #region IUnityStore

        event EventHandler PurchaseProcessed;

        IEnumerable<(string ProductId, (string Id, bool Consumable) NativeProduct)> ProductMap
        {
            get;
        }

        IEnumerable<(string SubscriptionId, string NativeProductId)> SubscriptionMap
        {
            get;
        }

        #endregion
    }
}
#endif
