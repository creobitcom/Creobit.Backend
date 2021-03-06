﻿#if CREOBIT_BACKEND_GOOGLEPLAY && CREOBIT_BACKEND_PLAYFAB && UNITY_ANDROID
using Creobit.Backend.Auth;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Creobit.Backend.Link
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
                                    CheckGoogleId();
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
                                    PlayFabErrorHandler.Process(error);

                                    onFailure();
                                });
                        }, onFailure);
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
                    GooglePlayPlayFabAuth.Logout(Login, ProcessException);
                }

                void Login()
                {
                    GooglePlayPlayFabAuth.Login(ProcessException, ProcessException);
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
        #region GooglePlayPlayFabLink

        private const string LinkKey = nameof(LinkKey);
        private const string LinkKeyExpirationTime = nameof(LinkKeyExpirationTime);

        private readonly IPlayFabLink PlayFabLink;
        private readonly IGooglePlayPlayFabAuth GooglePlayPlayFabAuth;

        private IExceptionHandler _exceptionHandler;
        private IPlayFabErrorHandler _playFabErrorHandler;

        public GooglePlayPlayFabLink(IPlayFabLink playFabLink, IGooglePlayPlayFabAuth googlePlayPlayFabAuth)
        {
            PlayFabLink = playFabLink;
            GooglePlayPlayFabAuth = googlePlayPlayFabAuth;
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
