#if CREOBIT_BACKEND_PLAYFAB
using Creobit.Backend.Auth;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Creobit.Backend.Link
{
    public sealed class CustomPlayFabLink : IPlayFabLink
    {
        #region ILink

        void ILink.Link(string linkKey, Action onComplete, Action onFailure)
        {
            var resultException = default(Exception);
            var sourceLoginResult = CustomPlayFabAuth.LoginResult;
            var targetLoginResult = default(LoginResult);

            LoginWithCustomId();

            void LoginWithCustomId()
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

                            UnlinkCustomId();
                        },
                        error =>
                        {
                            PlayFabErrorHandler.Process(error);

                            onFailure();
                        });
                }
                catch (Exception exception)
                {
                    ExceptionHandler.Process(exception);

                    onFailure();
                }
            }

            void UnlinkCustomId()
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
                            CheckLinkKeyExpirationTime();
                        },
                        error =>
                        {
                            PlayFabErrorHandler.Process(error);

                            onFailure();
                        });
                }
                catch (Exception exception)
                {
                    ExceptionHandler.Process(exception);

                    onFailure();
                }
            }

            void CheckLinkKeyExpirationTime()
            {
                try
                {
                    PlayFabClientAPI.GetUserData(
                        new GetUserDataRequest()
                        {
                            Keys = new List<string>()
                            {
                                LinkKeyExpirationTime
                            }
                        },
                        result =>
                        {
                            var data = result.Data;

                            if (data.TryGetValue(LinkKeyExpirationTime, out var record))
                            {
                                var now = DateTime.Now;
                                var expirationTime = DateTime.Parse(record.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

                                if (now > expirationTime)
                                {
                                    resultException = new Exception($"The \"{LinkKey}\" is out of date!");

                                    Restore();
                                }
                                else
                                {
                                    CheckCustomId();
                                }
                            }
                            else
                            {
                                resultException = new Exception($"The \"{LinkKeyExpirationTime}\" is not found!");

                                Restore();
                            }
                        },
                        error =>
                        {
                            PlayFabErrorHandler.Process(error);

                            onFailure();
                        });
                }
                catch (Exception exception)
                {
                    ExceptionHandler.Process(exception);

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
                            PlayFabErrorHandler.Process(error);

                            onFailure();
                        });
                }
                catch (Exception exception)
                {
                    ExceptionHandler.Process(exception);

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
                ExceptionHandler.Process(resultException);

                onFailure();
            }
        }

        void ILink.RequestLinkKey(int linkKeyLenght, Action<(string LinkKey, DateTime LinkKeyExpirationTime)> onComplete, Action onFailure) => PlayFabLink.RequestLinkKey(linkKeyLenght, onComplete, onFailure);

        #endregion
        #region CustomPlayFabLink

        private const string LinkKey = nameof(LinkKey);
        private const string LinkKeyExpirationTime = nameof(LinkKeyExpirationTime);

        private readonly IPlayFabLink PlayFabLink;
        private readonly ICustomPlayFabAuth CustomPlayFabAuth;

        private IExceptionHandler _exceptionHandler;
        private IPlayFabErrorHandler _playFabErrorHandler;

        public CustomPlayFabLink(IPlayFabLink playFabLink, ICustomPlayFabAuth customPlayFabAuth)
        {
            PlayFabLink = playFabLink;
            CustomPlayFabAuth = customPlayFabAuth;
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
