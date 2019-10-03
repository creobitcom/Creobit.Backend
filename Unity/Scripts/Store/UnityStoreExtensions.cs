#if CREOBIT_BACKEND_UNITY
namespace Creobit.Backend.Store
{
    public static class UnityStoreExtensions
    {
        #region UnityStoreExtensions

        public static string FindNativeProductId(this IUnityStore self, string productId)
        {
            foreach (var (ProductId, NativeProduct) in self.ProductMap)
            {
                if (ProductId == productId)
                {
                    return NativeProduct.Id;
                }
            }

            return null;
        }

        public static string FindProductId(this IUnityStore self, string nativeProductId)
        {
            foreach (var (ProductId, NativeProduct) in self.ProductMap)
            {
                if (NativeProduct.Id == nativeProductId)
                {
                    return ProductId;
                }
            }

            return null;
        }

        #endregion
    }
}
#endif
