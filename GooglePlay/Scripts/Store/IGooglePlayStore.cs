#if CREOBIT_BACKEND_GOOGLEPLAY && CREOBIT_BACKEND_UNITY && UNITY_ANDROID
using UnityEngine.Purchasing;

namespace Creobit.Backend.Store
{
    public interface IGooglePlayStore : IUnityStore
    {
        #region IGooglePlayStore

        IGooglePlayStoreExtensions GooglePlayStoreExtensions
        {
            get;
        }

        string PublicKey
        {
            get;
        }

        #endregion
    }
}
#endif
