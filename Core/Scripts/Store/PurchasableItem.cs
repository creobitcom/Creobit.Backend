using System;

namespace Creobit.Backend.Store
{
    internal abstract class PurchasableItem : IPurchasableItem
    {
        #region IPurchasable

        IPrice IPurchasable.Price => Price;

        void IPurchasable.Purchase(Action onComplete, Action onFailure) => PurchaseDelegate(this, onComplete, onFailure);

        #endregion

        #region IIdentifiable

        string IIdentifiable.Id => Id;

        #endregion

        #region PurchasableItem

        protected readonly string Id;
        protected readonly IPrice Price;

        protected PurchasableItem(string id, IPrice price)
        {
            Id = id;
            Price = price;
        }

        private Action<IPurchasableItem, Action, Action> _purchaseDelegate;

        public Action<IPurchasableItem, Action, Action> PurchaseDelegate
        {
            get => _purchaseDelegate ?? Purchase;
            set => _purchaseDelegate = value;
        }

        private void Purchase(IPurchasableItem product, Action onComplete, Action onFailure)
        {
            onFailure();
        }

        #endregion
    }
}
