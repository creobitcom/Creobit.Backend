#if CREOBIT_BACKEND_UNITY
using NativeProduct = UnityEngine.Purchasing.Product;

namespace Creobit.Backend.Store
{
    internal sealed class UnitySubscription : Subscription, IUnitySubscription
    {
        #region IUnitySubscription

        NativeProduct IUnitySubscription.NativeProduct => NativeProduct;

        #endregion
        #region UnitySubscription

        public UnitySubscription(string id, IPrice price) : base(id, price)
        {
        }

        public NativeProduct NativeProduct
        {
            get;
            set;
        }

        #endregion
    }
}
#endif
