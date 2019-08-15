using System;

namespace Creobit.Backend
{
    public interface IUser
    {
        #region IUser

        string UserName
        {
            get;
        }

        void SetUserName(string userName, Action onComplete, Action onFailure);

        #endregion
    }
}