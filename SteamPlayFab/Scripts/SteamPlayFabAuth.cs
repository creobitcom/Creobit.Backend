#if UNITY_EDITOR || UNITY_STANDALONE
#define ENABLED
#endif
#if ENABLED
using PlayFab;
using PlayFab.ClientModels;
using System;

namespace Creobit.Backend
{
    public sealed class SteamPlayFabAuth : ISteamPlayFabAuth
    {
        #region IAuth

        bool IAuth.IsLoggedIn => PlayFabAuth.IsLoggedIn && SteamAuth.IsLoggedIn;

        void IAuth.Login(Action onComplete, Action onFailure)
        {
            SteamLogin();

            void SteamLogin()
            {
                SteamAuth.Login(PlayFabLogin, onFailure);
            }

            void PlayFabLogin()
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

                            onComplete();
                        },
                        error =>
                        {
                            PlayFabErrorHandler?.Process(error);

                            SteamAuth.DestroyAuthSessionTicket(authSessionTicket);

                            onFailure();
                        });
                }
                catch (Exception exception)
                {
                    ExceptionHandler?.Process(exception);

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

        private readonly IPlayFabAuth PlayFabAuth;
        private readonly ISteamAuth SteamAuth;

        public SteamPlayFabAuth(IPlayFabAuth playFabAuth, ISteamAuth steamAuth)
        {
            PlayFabAuth = playFabAuth;
            SteamAuth = steamAuth;
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

        #endregion
    }
}
#else
using PlayFab.ClientModels;
using System;

namespace Creobit.Backend
{
    public sealed class SteamPlayFabAuth : ISteamPlayFabAuth
    {
        bool IAuth.IsLoggedIn => throw new NotSupportedException();

        void IAuth.Login(Action onComplete, Action onFailure) => throw new NotSupportedException();

        void IAuth.Logout(Action onComplete, Action onFailure) => throw new NotSupportedException();

        LoginResult IPlayFabAuth.LoginResult
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        string IPlayFabAuth.TitleId => throw new NotSupportedException();

        uint ISteamAuth.AppId => throw new NotSupportedException();

        string ISteamAuth.CreateAuthSessionTicket() => throw new NotSupportedException();

        void ISteamAuth.DestroyAuthSessionTicket(string authSessionTicket) => throw new NotSupportedException();

        public SteamPlayFabAuth(IPlayFabAuth playFabAuth, ISteamAuth steamAuth)
        {
        }
    }
}
#endif