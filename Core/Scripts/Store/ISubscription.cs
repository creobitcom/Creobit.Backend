using System;

namespace Creobit.Backend.Store
{
    public interface ISubscription : IIdentifiable, IPurchasable
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

        #endregion
    }
}
