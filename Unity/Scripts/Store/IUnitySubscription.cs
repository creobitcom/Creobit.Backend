#if CREOBIT_BACKEND_UNITY
using NativeProduct = UnityEngine.Purchasing.Product;

namespace Creobit.Backend.Store
{
    public interface IUnitySubscription : ISubscription
    {
        #region IUnitySubscription

        NativeProduct NativeProduct
        {
            get;
        }

        #endregion
    }
}
#endif
