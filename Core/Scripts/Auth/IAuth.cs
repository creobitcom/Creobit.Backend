using System;

namespace Creobit.Backend.Auth
{
    public interface IAuth
    {
        #region IAuth

        bool IsLoggedIn
        {
            get;
        }

        void Login(bool doCreateAccount, Action onComplete, Action onFailure);

        void Logout(Action onComplete, Action onFailure);

        #endregion
    }
}
