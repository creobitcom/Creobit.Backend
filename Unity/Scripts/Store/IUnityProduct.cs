#if CREOBIT_BACKEND_UNITY
using NativeProduct = UnityEngine.Purchasing.Product;

namespace Creobit.Backend.Store
{
    public interface IUnityProduct : IProduct
    {
        #region IUnityProduct

        NativeProduct NativeProduct
        {
            get;
        }

        #endregion
    }
}
#endif
