#if CREOBIT_BACKEND_UNITY
using NativeProduct = UnityEngine.Purchasing.Product;

namespace Creobit.Backend.Store
{
    internal sealed class UnityProduct : Product, IUnityProduct
    {
        #region IUnityProduct

        NativeProduct IUnityProduct.NativeProduct => NativeProduct;

        #endregion
        #region UnityProduct

        public UnityProduct(string id, IPrice price) : base(id, price)
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
