#if CREOBIT_BACKEND_PLAYFAB
using PlayFab;
using PlayFab.ClientModels;
using System;

namespace Creobit.Backend
{
    public sealed class PlayFabUser : IPlayFabUser
    {
        #region IUser

        string IUser.AvatarUrl => TitleInfo.AvatarUrl;

        string IUser.Name => TitleInfo.DisplayName;

        void IUser.Refresh(Action onComplete, Action onFailure)
        {
            try
            {
                PlayFabClientAPI.GetPlayerCombinedInfo(
                    new GetPlayerCombinedInfoRequest()
                    {
                        InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
                        {
                            GetUserAccountInfo = true
                        }
                    },
                    result =>
                    {
                        var loginResult = PlayFabAuth.LoginResult;

                        loginResult.InfoResultPayload = result.InfoResultPayload;

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

        void IUser.SetAvatarUrl(string avatarUrl, Action onComplete, Action onFailure)
        {
            try
            {
                PlayFabClientAPI.UpdateAvatarUrl(
                    new UpdateAvatarUrlRequest()
                    {
                        ImageUrl = avatarUrl
                    },
                    result =>
                    {
                        TitleInfo.AvatarUrl = avatarUrl;

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

        void IUser.SetName(string name, Action onComplete, Action onFailure)
        {
            try
            {
                PlayFabClientAPI.UpdateUserTitleDisplayName(
                    new UpdateUserTitleDisplayNameRequest()
                    {
                        DisplayName = name
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
#endif
