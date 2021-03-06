﻿#if CREOBIT_BACKEND_PLAYFAB
using PlayFab.ClientModels;

namespace Creobit.Backend.Auth
{
    public interface IPlayFabAuth : IAuth
    {
        #region IPlayFabAuth

        LoginResult LoginResult
        {
            get;
            set;
        }

        string TitleId
        {
            get;
        }

        #endregion
    }
}
#endif
