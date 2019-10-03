#if CREOBIT_BACKEND_GOOGLEPLAY && CREOBIT_BACKEND_UNITY && UNITY_ANDROID
using UnityEngine.Purchasing;

namespace Creobit.Backend.Store
{
    public sealed class GooglePlayStore : UnityStore, IGooglePlayStore
    {
        #region IGooglePlayStore

        string IGooglePlayStore.PublicKey => PublicKey;

        #endregion
        #region GooglePlayStore

        IGooglePlayStoreExtensions IGooglePlayStore.GooglePlayStoreExtensions => ExtensionProvider.GetExtension<IGooglePlayStoreExtensions>();

        private readonly string PublicKey;

        public GooglePlayStore(string publicKey)
        {
            PublicKey = publicKey;
        }

        protected override ConfigurationBuilder CreateConfigurationBuilder()
        {
            var configurationBuilder = base.CreateConfigurationBuilder();
            var googlePlayConfiguration = configurationBuilder.Configure<IGooglePlayConfiguration>();

            googlePlayConfiguration.SetPublicKey(PublicKey);

            return configurationBuilder;
        }

        #endregion
    }
}
#endif
