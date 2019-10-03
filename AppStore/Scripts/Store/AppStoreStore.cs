#if CREOBIT_BACKEND_APPSTORE && CREOBIT_BACKEND_UNITY && (UNITY_STANDALONE_OSX || UNITY_IOS)
using UnityEngine.Purchasing;

namespace Creobit.Backend.Store
{
    public sealed class AppStoreStore : UnityStore, IAppStoreStore
    {
        #region IAppStoreStore

        IAppleExtensions IAppStoreStore.AppleExtensions => ExtensionProvider.GetExtension<IAppleExtensions>();

        #endregion
    }
}
#endif
