﻿#if CREOBIT_BACKEND_IOS && CREOBIT_BACKEND_PLAYFAB && UNITY_IOS
using Creobit.Backend.Auth;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Creobit.Backend.Link
{
    public sealed class IosPlayFabLink : IPlayFabLink
    {
        #region ILink

        void ILink.Link(string linkKey, Action onComplete, Action onFailure)
        {
            var resultException = default(Exception);
            var sourceLoginResult = IosPlayFabAuth.LoginResult;
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
                            TitleId = IosPlayFabAuth.TitleId
                        },
                        result =>
                        {
                            targetLoginResult = result;
                            IosPlayFabAuth.LoginResult = result;

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
                                    CheckIosDeviceId();
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

            void CheckIosDeviceId()
            {
                var targetIosDeviceId = GetIosDeviceId(targetLoginResult);

                if (targetIosDeviceId == null || targetIosDeviceId == linkKey)
                {
                    LinkIosDeviceId();
                }
                else
                {
                    var sourceIosDeviceId = GetIosDeviceId(sourceLoginResult);

                    resultException = sourceIosDeviceId == targetIosDeviceId
                        ? new Exception("The source account already linked to the target account!")
                        : new Exception("The target account already has the linked account!");

                    Restore();
                }
            }

            string GetIosDeviceId(LoginResult loginResult)
            {
                var infoResultPayload = loginResult?.InfoResultPayload;
                var accountInfo = infoResultPayload?.AccountInfo;
                var iosDeviceInfo = accountInfo?.IosDeviceInfo;

                return iosDeviceInfo?.IosDeviceId;
            }

            void LinkIosDeviceId()
            {
                try
                {
                    PlayFabClientAPI.LinkIOSDeviceID(
                        new LinkIOSDeviceIDRequest()
                        {
                            DeviceId = SystemInfo.deviceUniqueIdentifier,
                            DeviceModel = SystemInfo.deviceModel,
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
                    IosPlayFabAuth.Logout(Login, ProcessException);
                }

                void Login()
                {
                    IosPlayFabAuth.Login(ProcessException, ProcessException);
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
        private readonly IIosPlayFabAuth IosPlayFabAuth;

        private IExceptionHandler _exceptionHandler;
        private IPlayFabErrorHandler _playFabErrorHandler;

        public IosPlayFabLink(IPlayFabLink playFabLink, IIosPlayFabAuth iosPlayFabAuth)
        {
            PlayFabLink = playFabLink;
            IosPlayFabAuth = iosPlayFabAuth;
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
