using System;

namespace Creobit.Backend
{
    public interface IAuth
    {
        #region IAuth

        bool IsLoggedIn
        {
            get;
        }

        void Login(Action onComplete, Action onFailure);

        void Logout(Action onComplete, Action onFailure);

        #endregion
    }
}