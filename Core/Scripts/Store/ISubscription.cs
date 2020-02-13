using System;

namespace Creobit.Backend.Store
{
    public interface ISubscription : IPurchasableItem
    {
        #region ISubscription

        DateTime? ExpireDate
        {
            get;
        }

        bool IsCanceled
        {
            get;
        }

        bool IsExpired
        {
            get;
        }

        bool IsSubscribed
        {
            get;
        }

        bool IsIntroductoryOffer
        {
            get;
        }

        #endregion
    }
}