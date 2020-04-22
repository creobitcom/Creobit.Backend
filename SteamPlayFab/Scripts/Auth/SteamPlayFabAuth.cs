#if CREOBIT_BACKEND_PLAYFAB && CREOBIT_BACKEND_STEAM && UNITY_STANDALONE
using PlayFab;
using PlayFab.ClientModels;
using System;

namespace Creobit.Backend.Auth
{
    public sealed class SteamPlayFabAuth : ISteamPlayFabAuth
    {
        #region IAuth

        bool IAuth.IsLoggedIn => PlayFabAuth.IsLoggedIn && SteamAuth.IsLoggedIn;

        void IAuth.Login(bool doCreateAccount, Action onComplete, Action onFailure)
        {
            SteamLogin(MaxLoginAttempts);

            void SteamLogin(int attemptsRemaining)
            {
                if (attemptsRemaining < 0)
                {
                    onFailure?.Invoke();
                    return;
                }

                SteamAuth.Login(() => PlayFabLogin(attemptsRemaining), onFailure);
            }

            void PlayFabLogin(int attemptsRemaining)
            {
                var authSessionTicket = SteamAuth.CreateAuthSessionTicket();

                PlayFabSettings.TitleId = PlayFabAuth.TitleId;

                try
                {
                    PlayFabClientAPI.LoginWithSteam(
                        new LoginWithSteamRequest()
                        {
                            CreateAccount = true,
                            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
                            {
                                GetUserAccountInfo = true
                            },
                            SteamTicket = authSessionTicket,
                            TitleId = PlayFabAuth.TitleId
                        },
                        result =>
                        {
                            PlayFabAuth.LoginResult = result;

                            SteamAuth.DestroyAuthSessionTicket(authSessionTicket);

                            onComplete?.Invoke();
                        },
                        error =>
                        {
                            PlayFabErrorHandler.Process(error);

                            SteamAuth.DestroyAuthSessionTicket(authSessionTicket);

                            if (error.Error == PlayFabErrorCode.InvalidSteamTicket)
                            {
                                SteamLogin(--attemptsRemaining);
                            }
                            else
                            {
                                onFailure?.Invoke();
                            }
                        });
                }
                catch (Exception exception)
                {
                    ExceptionHandler.Process(exception);

                    onFailure();
                }
            }
        }

        void IAuth.Logout(Action onComplete, Action onFailure)
        {
            PlayFabLogout();

            void PlayFabLogout()
            {
                PlayFabAuth.Logout(SteamLogout, onFailure);
            }

            void SteamLogout()
            {
                SteamAuth.Logout(onComplete, onFailure);
            }
        }

        #endregion
        #region IPlayFabAuth

        LoginResult IPlayFabAuth.LoginResult
        {
            get => PlayFabAuth.LoginResult;
            set => PlayFabAuth.LoginResult = value;
        }

        string IPlayFabAuth.TitleId => PlayFabAuth.TitleId;

        #endregion
        #region ISteamAuth

        uint ISteamAuth.AppId => SteamAuth.AppId;

        string ISteamAuth.CreateAuthSessionTicket() => SteamAuth.CreateAuthSessionTicket();

        void ISteamAuth.DestroyAuthSessionTicket(string authSessionTicket) => SteamAuth.DestroyAuthSessionTicket(authSessionTicket);

        #endregion
        #region SteamPlayFabAuth

        private const int MaxLoginAttempts = 3;

        private readonly IPlayFabAuth PlayFabAuth;
        private readonly ISteamAuth SteamAuth;

        private IExceptionHandler _exceptionHandler;
        private IPlayFabErrorHandler _playFabErrorHandler;

        public SteamPlayFabAuth(IPlayFabAuth playFabAuth, ISteamAuth steamAuth)
        {
            PlayFabAuth = playFabAuth;
            SteamAuth = steamAuth;
        }

        public IExceptionHandler ExceptionHandler
        {
            get => _exceptionHandler ?? Backend.ExceptionHandler.Default;
            set => _exceptionHandler = value;
        }

        public IPlayFabErrorHandler PlayFabErrorHandler
        {
            get => _playFabErrorHandler ?? Backend.PlayFabErrorHandler.Default;
            set => _playFabErrorHandler = value;
        }

        #endregion
    }
}
#endif
