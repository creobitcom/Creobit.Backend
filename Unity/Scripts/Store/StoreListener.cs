#if CREOBIT_BACKEND_UNITY
using System;
using UnityEngine.Purchasing;
using NativeProduct = UnityEngine.Purchasing.Product;

namespace Creobit.Backend.Store
{
    public abstract partial class UnityStore
    {
        private sealed class StoreListener : IStoreListener
        {
            #region IStoreListener

            void IStoreListener.OnInitialized(IStoreController controller, IExtensionProvider provider)
            {
                var eventArgs = new InitializedEventArgs(controller, provider);

                Initialized(this, eventArgs);
            }

            void IStoreListener.OnInitializeFailed(InitializationFailureReason reason)
            {
                var eventArgs = new InitializeFailedEventArgs(reason);

                InitializeFailed(this, eventArgs);
            }

            void IStoreListener.OnPurchaseFailed(NativeProduct product, PurchaseFailureReason reason)
            {
                var eventArgs = new PurchaseFailedEventArgs(product, reason);

                PurchaseFailed(this, eventArgs);
            }

            PurchaseProcessingResult IStoreListener.ProcessPurchase(PurchaseEventArgs purchaseEventArgs)
            {
                var eventArgs = new ProcessPurchaseEventArgs(purchaseEventArgs.purchasedProduct);

                ProcessPurchase(this, eventArgs);

                return PurchaseProcessingResult.Complete;
            }

            #endregion
            #region StoreListener

            public event EventHandler<InitializedEventArgs> Initialized = delegate { };

            public event EventHandler<InitializeFailedEventArgs> InitializeFailed = delegate { };

            public event EventHandler<PurchaseFailedEventArgs> PurchaseFailed = delegate { };

            public event EventHandler<ProcessPurchaseEventArgs> ProcessPurchase = delegate { };

            #endregion
        }
    }
}
#endif