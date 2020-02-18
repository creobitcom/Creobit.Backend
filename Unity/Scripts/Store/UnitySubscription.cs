#if CREOBIT_BACKEND_UNITY
using NativeProduct = UnityEngine.Purchasing.Product;

namespace Creobit.Backend.Store
{
    internal sealed partial class UnitySubscription : Subscription, IUnitySubscription
    {
        #region IUnitySubscription

        public NativeProduct NativeProduct
        {
            get;
            set;
        }

        #endregion
        #region UnitySubscription

        public UnitySubscription(string id, IPrice price) : base(id, price)
        {
        }

        #endregion
    }
}
#endif