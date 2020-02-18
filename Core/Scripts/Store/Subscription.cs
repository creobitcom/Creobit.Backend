using System;
using System.Text;

namespace Creobit.Backend.Store
{
    internal class Subscription : PurchasableItem, ISubscription
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
        #region ISubscription

        DateTime? ISubscription.ExpireDate => GetExpireDateDelegate(this);

        bool ISubscription.IsCanceled => IsCanceledDelegate(this);

        bool ISubscription.IsExpired => IsExpiredDelegate(this);

        bool ISubscription.IsSubscribed => IsSubscribedDelegate(this);

        bool ISubscription.IsIntroductoryOffer => IsTrialDelegate(this);

        #endregion
        #region Subscription

        public Subscription(string id, IPrice price) : base(id, price)
        {

        }

        private Func<ISubscription, DateTime?> _getExpireDateDelegate;
        private Func<ISubscription, bool> _isCanceledDelegate;
        private Func<ISubscription, bool> _isExpiredDelegate;
        private Func<ISubscription, bool> _isSubscribedDelegate;
        private Func<ISubscription, bool> _isTrialDelegate;

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

        public Func<ISubscription, bool> IsTrialDelegate
        {
            get => _isTrialDelegate ?? IsTrial;
            set => _isTrialDelegate = value;
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

        private bool IsTrial(ISubscription subscription)
        {
            return false;
        }

        #endregion
    }
}
