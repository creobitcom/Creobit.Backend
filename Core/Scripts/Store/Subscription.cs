using System;
using System.Text;

namespace Creobit.Backend.Store
{
    internal class Subscription : ISubscription
    {
        #region Object

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append($"{{ ");
            stringBuilder.Append($"{nameof(Id)}:{Id} ");
            stringBuilder.Append($"{nameof(Price)}:{Price} ");
            stringBuilder.Append($"{nameof(ISubscription.ExpireDate)}:{((ISubscription)this).ExpireDate} ");
            stringBuilder.Append($"{nameof(ISubscription.IsCanceled)}:{((ISubscription)this).IsCanceled} ");
            stringBuilder.Append($"{nameof(ISubscription.IsExpired)}:{((ISubscription)this).IsExpired} ");
            stringBuilder.Append($"{nameof(ISubscription.IsSubscribed)}:{((ISubscription)this).IsSubscribed} ");
            stringBuilder.Append($"}}");

            return stringBuilder.ToString();
        }

        #endregion
        #region IIdentifiable

        string IIdentifiable.Id => Id;

        #endregion
        #region IPurchasable

        IPrice IPurchasable.Price => Price;

        void IPurchasable.Purchase(Action onComplete, Action onFailure) => PurchaseDelegate(this, onComplete, onFailure);

        #endregion
        #region ISubscription

        DateTime? ISubscription.ExpireDate => GetExpireDateDelegate(this);

        bool ISubscription.IsCanceled => IsCanceledDelegate(this);

        bool ISubscription.IsExpired => IsExpiredDelegate(this);

        bool ISubscription.IsSubscribed => IsSubscribedDelegate(this);

        #endregion
        #region Subscription

        public readonly string Id;
        public readonly IPrice Price;

        private Func<ISubscription, DateTime?> _getExpireDateDelegate;
        private Func<ISubscription, bool> _isCanceledDelegate;
        private Func<ISubscription, bool> _isExpiredDelegate;
        private Func<ISubscription, bool> _isSubscribedDelegate;
        private Action<ISubscription, Action, Action> _purchaseDelegate;

        public Subscription(string id, IPrice price)
        {
            Id = id;
            Price = price;
        }

        public Func<ISubscription, DateTime?> GetExpireDateDelegate
        {
            get => _getExpireDateDelegate ?? GetExpireDate;
            set => _getExpireDateDelegate = value;
        }

        public Func<ISubscription, bool> IsCanceledDelegate
        {
            get => _isCanceledDelegate ?? IsCanceled;
            set => _isCanceledDelegate = value;
        }

        public Func<ISubscription, bool> IsExpiredDelegate
        {
            get => _isExpiredDelegate ?? IsExpired;
            set => _isExpiredDelegate = value;
        }

        public Func<ISubscription, bool> IsSubscribedDelegate
        {
            get => _isSubscribedDelegate ?? IsSubscribed;
            set => _isSubscribedDelegate = value;
        }

        public Action<ISubscription, Action, Action> PurchaseDelegate
        {
            get => _purchaseDelegate ?? Purchase;
            set => _purchaseDelegate = value;
        }

        private DateTime? GetExpireDate(ISubscription subscription)
        {
            return null;
        }

        private bool IsCanceled(ISubscription subscription)
        {
            return false;
        }

        private bool IsExpired(ISubscription subscription)
        {
            return false;
        }

        private bool IsSubscribed(ISubscription subscription)
        {
            return false;
        }

        private void Purchase(ISubscription subscription, Action onComplete, Action onFailure)
        {
            onFailure();
        }

        #endregion
    }
}
