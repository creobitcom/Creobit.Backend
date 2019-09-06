#if CREOBIT_BACKEND_GOOGLEPLAY && CREOBIT_BACKEND_PLAYFAB && UNITY_ANDROID
using Creobit.Backend.Auth;
using PlayFab;
using PlayFab.ClientModels;
using System;

namespace Creobit.Backend
{
    public sealed class GooglePlayPlayFabLink : IPlayFabLink
    {
        #region ILink

        void ILink.Link(string linkKey, Action onComplete, Action onFailure)
        {
            var resultException = default(Exception);
            var sourceLoginResult = GooglePlayPlayFabAuth.LoginResult;
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
                            TitleId = GooglePlayPlayFabAuth.TitleId
                        },
                        result =>
                        {
                            targetLoginResult = result;
                            GooglePlayPlayFabAuth.LoginResult = result;

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
                            CheckGoogleId();
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

            void CheckGoogleId()
            {
                var targetGoogleId = GetGoogleId(targetLoginResult);

                if (targetGoogleId == null)
                {
                    LinkGoogleAccount();
                }
                else
                {
                    var sourceGoogleId = GetGoogleId(sourceLoginResult);

                    resultException = sourceGoogleId == targetGoogleId
                        ? new Exception("The source account already linked to the target account!")
                        : new Exception("The target account already has the linked account!");

                    Restore();
                }
            }

            string GetGoogleId(LoginResult loginResult)
            {
                var infoResultPayload = loginResult?.InfoResultPayload;
                var accountInfo = infoResultPayload?.AccountInfo;
                var googleInfo = accountInfo?.GoogleInfo;

                return googleInfo?.GoogleId;
            }

            void LinkGoogleAccount()
            {
                try
                {
                    GooglePlayPlayFabAuth.GetServerAuthCode(
                        serverAuthCode =>
                        {
                            PlayFabClientAPI.LinkGoogleAccount(
                                new LinkGoogleAccountRequest()
                                {
                                    ForceLink = true,
                                    ServerAuthCode = serverAuthCode
                                },
                                result =>
                                {
                                    onComplete();
                                },
                                error =>
                                {
                                    PlayFabErrorHandler?.Process(error);

                                    onFailure();
                                });
                        }, onFailure);
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
                    GooglePlayPlayFabAuth.Logout(Login, ProcessException);
                }

                void Login()
                {
                    GooglePlayPlayFabAuth.Login(ProcessException, ProcessException);
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
        #region GooglePlayPlayFabLink

        private readonly IPlayFabLink PlayFabLink;
        private readonly IGooglePlayPlayFabAuth GooglePlayPlayFabAuth;

        public GooglePlayPlayFabLink(IPlayFabLink playFabLink, IGooglePlayPlayFabAuth googlePlayPlayFabAuth)
        {
            PlayFabLink = playFabLink;
            GooglePlayPlayFabAuth = googlePlayPlayFabAuth;
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
