using PlayFab;
using PlayFab.ClientModels;
using System;

namespace Creobit.Backend
{
    public sealed class CustomPlayFabLink : IPlayFabLink
    {
        #region ILink

        void ILink.Link(string linkKey, Action onComplete, Action onFailure)
        {
            var resultException = default(Exception);
            var sourceLoginResult = CustomPlayFabAuth.LoginResult;
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
                            TitleId = CustomPlayFabAuth.TitleId
                        },
                        result =>
                        {
                            targetLoginResult = result;
                            CustomPlayFabAuth.LoginResult = result;

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
                            CheckCustomId();
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

            void CheckCustomId()
            {
                var targetCustomId = GetCustomId(targetLoginResult);

                if (targetCustomId == null || targetCustomId == linkKey)
                {
                    LinkCustomID();
                }
                else
                {
                    var sourceCustomId = GetCustomId(sourceLoginResult);

                    resultException = sourceCustomId == targetCustomId
                        ? new Exception("The source account already linked to the target account!")
                        : new Exception("The target account already has the linked account!");

                    Restore();
                }
            }

            string GetCustomId(LoginResult loginResult)
            {
                var infoResultPayload = loginResult?.InfoResultPayload;
                var accountInfo = infoResultPayload?.AccountInfo;
                var customIdInfo = accountInfo?.CustomIdInfo;

                return customIdInfo?.CustomId;
            }

            void LinkCustomID()
            {
                try
                {
                    var customId = GetCustomId(sourceLoginResult);

                    PlayFabClientAPI.LinkCustomID(
                        new LinkCustomIDRequest()
                        {
                            CustomId = customId,
                            ForceLink = true
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
                    CustomPlayFabAuth.Logout(Login, ProcessException);
                }

                void Login()
                {
                    CustomPlayFabAuth.Login(ProcessException, ProcessException);
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
        #region CustomPlayFabLink

        private readonly IPlayFabLink PlayFabLink;
        private readonly ICustomPlayFabAuth CustomPlayFabAuth;

        public CustomPlayFabLink(IPlayFabLink playFabLink, ICustomPlayFabAuth customPlayFabAuth)
        {
            PlayFabLink = playFabLink;
            CustomPlayFabAuth = customPlayFabAuth;
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