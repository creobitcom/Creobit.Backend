#if CREOBIT_BACKEND_PLAYFAB && CREOBIT_BACKEND_STEAM && UNITY_STANDALONE
using PlayFab;
using PlayFab.ClientModels;
using System;

namespace Creobit.Backend
{
    public sealed class SteamPlayFabLink : IPlayFabLink
    {
        #region ILink

        void ILink.Link(string linkKey, Action onComplete, Action onFailure)
        {
            var resultException = default(Exception);
            var sourceLoginResult = SteamPlayFabAuth.LoginResult;
            var targetLoginResult = default(LoginResult);

            LoginWithCustomID();

            void LoginWithCustomID()
            {
                try
                {
                    PlayFabClientAPI.LoginWithCustomID(
                        new LoginWithCustomIDRequest()
                        {
                            CreateAccount = false,
                            CustomId = linkKey,
                            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
                            {
                                GetUserAccountInfo = true
                            },
                            TitleId = SteamPlayFabAuth.TitleId
                        },
                        result =>
                        {
                            targetLoginResult = result;
                            SteamPlayFabAuth.LoginResult = result;

                            UnlinkCustomID();
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

            void UnlinkCustomID()
            {
                try
                {
                    PlayFabClientAPI.UnlinkCustomID(
                        new UnlinkCustomIDRequest()
                        {
                            CustomId = linkKey
                        },
                        result =>
                        {
                            CheckSteamId();
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

            void CheckSteamId()
            {
                var targetSteamId = GetSteamId(targetLoginResult);

                if (targetSteamId == null)
                {
                    LinkSteamAccount();
                }
                else
                {
                    var sourceSteamId = GetSteamId(sourceLoginResult);

                    resultException = sourceSteamId == targetSteamId
                        ? new Exception("The source account already linked to the target account!")
                        : new Exception("The target account already has the linked account!");

                    Restore();
                }
            }

            string GetSteamId(LoginResult loginResult)
            {
                var infoResultPayload = loginResult?.InfoResultPayload;
                var accountInfo = infoResultPayload?.AccountInfo;
                var steamId = accountInfo?.SteamInfo;

                return steamId?.SteamId;
            }

            void LinkSteamAccount()
            {
                try
                {
                    var authSessionTicket = SteamPlayFabAuth.CreateAuthSessionTicket();

                    PlayFabClientAPI.LinkSteamAccount(
                        new LinkSteamAccountRequest()
                        {
                            ForceLink = true,
                            SteamTicket = authSessionTicket
                        },
                        result =>
                        {
                            SteamPlayFabAuth.DestroyAuthSessionTicket(authSessionTicket);

                            onComplete();
                        },
                        error =>
                        {
                            SteamPlayFabAuth.DestroyAuthSessionTicket(authSessionTicket);
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

            void Restore()
            {
                Logout();

                void Logout()
                {
                    SteamPlayFabAuth.Logout(Login, ProcessException);
                }

                void Login()
                {
                    SteamPlayFabAuth.Login(ProcessException, ProcessException);
                }
            }

            void ProcessException()
            {
                ExceptionHandler?.Process(resultException);

                onFailure();
            }
        }

        void ILink.RequestLinkKey(int linkKeyLenght, Action<string> onComplete, Action onFailure) => PlayFabLink.RequestLinkKey(linkKeyLenght, onComplete, onFailure);

        #endregion
        #region SteamPlayFabLink

        private readonly IPlayFabLink PlayFabLink;
        private readonly ISteamPlayFabAuth SteamPlayFabAuth;

        public SteamPlayFabLink(IPlayFabLink playFabLink, ISteamPlayFabAuth steamPlayFabAuth)
        {
            PlayFabLink = playFabLink;
            SteamPlayFabAuth = steamPlayFabAuth;
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
#endif
