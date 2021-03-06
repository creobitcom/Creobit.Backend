﻿#if CREOBIT_BACKEND_ANDROID && CREOBIT_BACKEND_PLAYFAB && UNITY_ANDROID
using Creobit.Backend.Auth;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Creobit.Backend.Link
{
    public sealed class AndroidPlayFabLink : IPlayFabLink
    {
        #region ILink

        void ILink.Link(string linkKey, Action onComplete, Action onFailure)
        {
            var resultException = default(Exception);
            var sourceLoginResult = AndroidPlayFabAuth.LoginResult;
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
                            TitleId = AndroidPlayFabAuth.TitleId
                        },
                        result =>
                        {
                            targetLoginResult = result;
                            AndroidPlayFabAuth.LoginResult = result;

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
                                    CheckAndroidDeviceId();
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

            void CheckAndroidDeviceId()
            {
                var targetAndroidDeviceId = GetAndroidDeviceID(targetLoginResult);

                if (targetAndroidDeviceId == null || targetAndroidDeviceId == linkKey)
                {
                    LinkAndroidDeviceID();
                }
                else
                {
                    var sourceAndroidDeviceId = GetAndroidDeviceID(sourceLoginResult);

                    resultException = sourceAndroidDeviceId == targetAndroidDeviceId
                        ? new Exception("The source account already linked to the target account!")
                        : new Exception("The target account already has the linked account!");

                    Restore();
                }
            }

            string GetAndroidDeviceID(LoginResult loginResult)
            {
                var infoResultPayload = loginResult?.InfoResultPayload;
                var accountInfo = infoResultPayload?.AccountInfo;
                var androidDeviceInfo = accountInfo?.AndroidDeviceInfo;

                return androidDeviceInfo?.AndroidDeviceId;
            }

            void LinkAndroidDeviceID()
            {
                try
                {
                    PlayFabClientAPI.LinkAndroidDeviceID(
                        new LinkAndroidDeviceIDRequest()
                        {
                            AndroidDeviceId = SystemInfo.deviceUniqueIdentifier,
                            AndroidDevice = SystemInfo.deviceModel,
                            ForceLink = true,
                            OS = SystemInfo.operatingSystem
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
                    AndroidPlayFabAuth.Logout(Login, ProcessException);
                }

                void Login()
                {
                    AndroidPlayFabAuth.Login(ProcessException, ProcessException);
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
        #region IosPlayFabLink

        private const string LinkKey = nameof(LinkKey);
        private const string LinkKeyExpirationTime = nameof(LinkKeyExpirationTime);

        private readonly IPlayFabLink PlayFabLink;
        private readonly IAndroidPlayFabAuth AndroidPlayFabAuth;

        private IExceptionHandler _exceptionHandler;
        private IPlayFabErrorHandler _playFabErrorHandler;

        public AndroidPlayFabLink(IPlayFabLink playFabLink, IAndroidPlayFabAuth androidPlayFabAuth)
        {
            PlayFabLink = playFabLink;
            AndroidPlayFabAuth = androidPlayFabAuth;
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
