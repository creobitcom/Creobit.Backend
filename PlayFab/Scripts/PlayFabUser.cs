using PlayFab;
using PlayFab.ClientModels;
using System;

namespace Creobit.Backend
{
    public sealed class PlayFabUser : IPlayFabUser
    {
        #region IUser

        string IUser.UserName => TitleInfo.DisplayName;

        void IUser.SetUserName(string userName, Action onComplete, Action onFailure)
        {
            try
            {
                PlayFabClientAPI.UpdateUserTitleDisplayName(
                    new UpdateUserTitleDisplayNameRequest()
                    {
                        DisplayName = userName
                    },
                    result =>
                    {
                        TitleInfo.DisplayName = result.DisplayName;

                        onComplete();
                    },
                    error =>
                    {
                        PlayFabErrorHandler?.Process(error);

                        onFailure();
                    });
            }
            catch (Exception exception)
            {
                ExceptionHandler?.Process(exception);

                onFailure();
            }
        }

        #endregion
        #region PlayFabUser

        private readonly IPlayFabAuth PlayFabAuth;

        public PlayFabUser(IPlayFabAuth playFabAuth)
        {
            PlayFabAuth = playFabAuth;
        }

        public IExceptionHandler ExceptionHandler
        {
            get;
            set;
        } = Backend.ExceptionHandler.Default;

        public IPlayFabErrorHandler PlayFabErrorHandler
        {
            get;
            set;
        } = Backend.PlayFabErrorHandler.Default;

        private UserTitleInfo TitleInfo
        {
            get
            {
                var loginResult = PlayFabAuth.LoginResult;
                var infoResultPayload = loginResult.InfoResultPayload;
                var accountInfo = infoResultPayload.AccountInfo;

                return accountInfo.TitleInfo;
            }
        }

        #endregion
    }
}