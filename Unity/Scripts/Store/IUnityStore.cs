#if CREOBIT_BACKEND_UNITY
using System.Collections.Generic;

namespace Creobit.Backend.Store
{
    public interface IUnityStore : IStore
    {
        #region IUnityStore

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
