#if CREOBIT_BACKEND_APPSTORE && CREOBIT_BACKEND_UNITY && (UNITY_STANDALONE_OSX || UNITY_IOS)
using UnityEngine.Purchasing;

namespace Creobit.Backend.Store
{
    public interface IAppStoreStore : IUnityStore
    {
        #region IAppStoreStore

        IAppleExtensions AppleExtensions
        {
            get;
        }

        #endregion
    }
}
#endif
